using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Admin
    {
        public int ID { get; set; }
        public string ObjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
        public string Status { get; set; } = "uninvited";
    }
}
