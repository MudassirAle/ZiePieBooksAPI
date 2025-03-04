using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class PreOrder
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string? QBDesktopVersion { get; set; }
        public bool ExistingAccounting { get; set; }
        public DateTime? LatestAccountingDate { get; set; }
        public int NumBankAccounts { get; set; }
        public int NumCreditCards { get; set; }
        public bool IncludeChecks { get; set; }
        public int ChecksPerMonth { get; set; }
        public bool IsPaymentMade { get; set; }
    }

    public class PaymentDTO
    {
        public bool IsPaymentMade { get; set; }
        public string PaymentId { get; set; } = string.Empty;
    }
}
