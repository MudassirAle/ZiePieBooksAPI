using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class SubTenant
    {
        public int ID { get; set; }
        public int TenantId { get; set; }
        public string ObjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public SubTenantPermission Permissions { get; set; } = new SubTenantPermission();
        public string Status { get; set; } = "uninvited";
    }
    public class SubTenantPermission
    {
        public bool CanViewStatements { get; set; } = false;
        public bool CanRefreshStatements { get; set; } = false;
    }
}
