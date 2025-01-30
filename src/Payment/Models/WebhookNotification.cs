namespace Payment.Models
{
    public class WebhookNotification
    {
        public string Action { get; set; }
        public string api_version { get; set; }
        public NotificationData data { get; set; }
        public string date_created { get; set; }
        public long Id { get; set; }
        public bool LiveMode { get; set; }
        public string user_id { get; set; }
    }
}
