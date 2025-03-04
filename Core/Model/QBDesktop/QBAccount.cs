namespace Core.Model.QBDesktop
{
    public class QBAccount
    {
        public int Id { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public string ListID { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
