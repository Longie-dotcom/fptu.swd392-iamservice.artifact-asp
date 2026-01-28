namespace SWD392.MessageBroker
{
    public class CollectionReportStatusUpdateDTO
    {
        public Guid CollectionReportID { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
