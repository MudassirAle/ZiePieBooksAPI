using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.QBDesktop
{
    public class QBTransaction
    {
        public string AccountTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Class { get; set; }
        public string? CheckNo { get; set; }
        public decimal Amount { get; set; }
    }
}
