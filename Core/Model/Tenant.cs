using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
	public class Tenant
	{
        public int ID { get; set; }
        public string ObjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string CompanyPhone { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
		public List<string> Products { get; set; } = new List<string>();
        public string Status { get; set; } = "uninvited";
        public string? PaymentMethod { get; set; }
        public string? TeamMember { get; set; }
    }
}