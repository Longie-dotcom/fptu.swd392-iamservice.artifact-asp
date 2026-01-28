namespace SWD392.MessageBroker
{
    public class CollectionTaskStatusUpdateDTO
    {
        public Guid CollectionReportID { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
