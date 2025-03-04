using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
	public class Customer
	{
		public int ID { get; set; }
        public int TenantId { get; set; }
        public string ObjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Service { get; set; } = string.Empty;
        public string Status { get; set; } = "uninvited";
		public string Platform { get; set; } = string.Empty;
		public string PlaidConnectedBy { get; set; } = string.Empty;
    }
}
