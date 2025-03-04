using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Account
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
    }
}
