using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Model.Plaid
{
	public class PlaidAccountDTO
	{
		public int BusinessId { get; set; }
		public string ItemId { get; set; } = string.Empty;
		public string InstitutionId { get; set; } = string.Empty;
		public string AccessToken { get; set; } = string.Empty;
		public List<PlaidBankAccount> PlaidBankAccount { get; set; } = new List<PlaidBankAccount>();
		public DateTime LinkedAt { get; set; }
		public string LinkedBy { get; set; } = string.Empty;
		public int LinkedById { get; set; }
        public bool ShareWithTenant { get; set; } = false;
        public bool ShareWithCustomer { get; set; } = false;
        public string Status { get; set; } = string.Empty;
	}
	public class PlaidAccount
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string ItemId { get; set; } = string.Empty;
        public string InstitutionId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public PlaidBankAccount PlaidBankAccount { get; set; } = new PlaidBankAccount();
        public DateTime LinkedAt { get; set; }
        public string LinkedBy { get; set; } = string.Empty;
        public int LinkedById { get; set; }
        public bool ShareWithTenant { get; set; }
		public bool ShareWithCustomer { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    public class UpdatePlaidAccountDTO
    {
        public string ItemId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }

    public class WebhookModel
    {
        public string environment { get; set; } = string.Empty;
        public string? error { get; set; }
        public string item_id { get; set; } = string.Empty;
        public int new_transactions { get; set; }
        public string webhook_code { get; set; } = string.Empty;
        public string webhook_type { get; set; } = string.Empty;
    }
}