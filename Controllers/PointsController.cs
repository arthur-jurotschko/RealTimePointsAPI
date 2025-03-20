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
                int points = CalculatePoints(transaction.Amount);

                UpdatePointsBalance(transaction.CustomerId, points);

                RecordTransaction(transaction, points);

                return Ok(new { message = "Pontos creditados com sucesso!", points });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

        private int CalculatePoints(decimal amount)
        {
            // Exemplo: 1 ponto a cada 10 reais gastos
            return (int)(amount / 10);
        }

        private void UpdatePointsBalance(int customerId, int points)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "UPDATE PONTOS SET saldo_de_pontos = saldo_de_pontos + @Points WHERE id_pessoa_que_consumiu = @CustomerId";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Points", points);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private void RecordTransaction(Transaction transaction, int points)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string query = "INSERT INTO CONSUMO (id_pessoa_que_consumiu, valor_total, pontos_obtidos_na_transacao, ProcessadoTempoReal) " +
                           "VALUES (@CustomerId, @Amount, @Points, 1)";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", transaction.CustomerId);
            command.Parameters.AddWithValue("@Amount", transaction.Amount);
            command.Parameters.AddWithValue("@Points", points);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
