namespace SWD392.MessageBroker
{
    public class CollectorProfileDTO
    {
        public Guid UserID { get; set; }
        public string ContactInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
