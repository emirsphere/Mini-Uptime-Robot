namespace Mini_Uptime_Robot.Models
{
    public class WebsiteResult
    {
        public int Latency { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
