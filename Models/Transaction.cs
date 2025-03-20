namespace RealTimePointsAPI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public int PointsEarned { get; set; }
        public bool ProcessedRealTime { get; set; } = false;
    }
}
