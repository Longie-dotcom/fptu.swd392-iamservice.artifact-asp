namespace SWD392.MessageBroker
{
    public class IncentiveRewardDTO
    {
        public Guid CollectionReportID { get; set; }
        public int Point { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
