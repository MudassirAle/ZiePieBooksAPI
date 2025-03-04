namespace Core.Model
{
    public class QBManualAccount
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public int OnboardingId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public DateTime OpeningDate { get; set; }
    }
    public class QBManualAccountResponse
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public int OnboardingId { get; set; }
        public string AccountSubType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public DateTime OpeningDate { get; set; }
    }
}