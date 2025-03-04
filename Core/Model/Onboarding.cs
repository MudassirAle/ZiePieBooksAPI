using Newtonsoft.Json;

namespace Core.Model
{
    public class Onboarding
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public int StepNumber { get; set; }
        public string StepName { get; set; } = string.Empty;
        public DataSource? DataSource { get; set; }
    }
    public class OnboardingDTO
    {
        public int BusinessId { get; set; }
        public int StepNumber { get; set; }
        public string StepName { get; set; } = string.Empty;
    }
    public class DataSourceDTO
    {
        public int ID { get; set; }
        public int StepNumber { get; set; }
        public string StepName { get; set; } = string.Empty;
        public DataSource DataSource { get; set; } = new DataSource();
    }
    public class DataSource
    {
        public int PlatformAccountId { get; set; }
        public bool IsPlaid { get; set; }
        public int? PlaidAccountId { get; set; }
        public string? StatementUrl { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
    public class OnboardingForAdmin
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public Hierarchy Hierarchy { get; set; } = new Hierarchy();
        public string Platform { get; set; } = string.Empty;
        public DateTime? ReceivedAt { get; set; }
        public string? Status { get; set; } = string.Empty;
        public int? AssignedToId { get; set; }
    }
    public class AssignmentDTO
    {
        public int ID { get; set; }
        public int AssignedToId { get; set; }
    }
    public class OnboardingForSubAdmin
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public Hierarchy Hierarchy { get; set; } = new Hierarchy();
        public string Platform { get; set; } = string.Empty;
        public DateTime? ReceivedAt { get; set; }
        public string? Status { get; set; } = string.Empty;
        public string? FileAddress { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}