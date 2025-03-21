using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RealTimePointsAPI.Models;

namespace RealTimePointsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PointsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("credit-points")]
        public IActionResult CreditPoints([FromBody] Transaction transaction)
        {
            try
            {
                EnsureCustomerExists(transaction.CustomerId); // Verifica se o cliente existe
                int points = CalculatePoints(transaction.Amount);
                UpdatePointsBalance(transaction.CustomerId, points);
                RecordConsumption(transaction.CustomerId, transaction.Amount, points);

                // Registrar entrada na tabela MEMORIAL
                RecordMemorialEntry(transaction.TransactionId, points);

                return Ok(new { message = "Pontos creditados em tempo real com sucesso!", points });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

        private void RecordMemorialEntry(int transactionId, int points)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "INSERT INTO MEMORIAL (id_transacao, pontos_calculados, data_processo) " +
                           "VALUES (@TransactionId, @Points, GETDATE())";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TransactionId", transactionId);
            command.Parameters.AddWithValue("@Points", points);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private void EnsureCustomerExists(int customerId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "IF NOT EXISTS (SELECT 1 FROM PONTOS WHERE id_pessoa_que_consumiu = @CustomerId) " +
                           "INSERT INTO PONTOS (id_pessoa_que_consumiu, saldo_de_pontos, data_atualizacao) " +
                           "VALUES (@CustomerId, 0, GETDATE())";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private int CalculatePoints(decimal amount)
        {
            return (int)(amount / 10); // 1 ponto a cada R$10,00
        }

        private void UpdatePointsBalance(int customerId, int points)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "UPDATE PONTOS SET saldo_de_pontos = saldo_de_pontos + @Points, data_atualizacao = GETDATE() " +
                           "WHERE id_pessoa_que_consumiu = @CustomerId";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Points", points);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private void RecordConsumption(int customerId, decimal amount, int points)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "INSERT INTO CONSUMO (id_pessoa_que_consumiu, valor_total, pontos_obtidos_na_transacao, ProcessadoTempoReal) " +
                           "VALUES (@CustomerId, @Amount, @Points, 1)";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Points", points);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
