namespace NT118_Server_API.Models
{
    public class NotificationManagerData
    {
        public int? ID { get; set; }
        public string? Phban { get; set; }
        public string? Ngnhan { get; set; }
        public string? Date { get; set; }
        public string? Content { get; set; }
        public int? Received { get; set; }
        public int? Seen { get; set; }
        public bool? IsSeen { get; set; }
    }
}
