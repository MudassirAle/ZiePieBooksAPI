namespace Core.Model
{
    public class SubAdmin
    {
        public int ID { get; set; }
        public int AdminId { get; set; }
        public string ObjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public SubAdminPermission Permissions { get; set; } = new SubAdminPermission();
        public string Status { get; set; } = "uninvited";
    }
    public class SubAdminPermission
    {
        public bool CanViewStatements { get; set; } = false;
        public bool CanRefreshStatements { get; set; } = false;
    }
}
