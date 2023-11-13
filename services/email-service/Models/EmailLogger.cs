namespace email_service.Models
{
    public class EmailLogger
    {
        public int Int { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime EmailSent { get; set; }
    }
}