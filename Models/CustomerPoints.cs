namespace RealTimePointsAPI.Models
{
    public class CustomerPoints
    {
        public int CustomerId { get; set; }
        public int PointsBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
