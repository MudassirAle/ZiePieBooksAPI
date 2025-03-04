using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Plaid
{
    public class PlaidStatement
    {
        public int ID { get; set; }
        public int AccountId { get; set; }
        public string StatementId { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string BlobUri { get; set; } = string.Empty;
    }
}
