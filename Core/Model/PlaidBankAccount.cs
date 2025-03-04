using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Model
{
	public class PlaidBankAccount
	{
		[JsonPropertyName("account_id")]
		public string? AccountId { get; set; } = string.Empty;

		[JsonPropertyName("balances")]
		public Balances Balances { get; set; } = new Balances();

		[JsonPropertyName("mask")]
		public string? Mask { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string? Name { get; set; } = string.Empty;

		[JsonPropertyName("official_name")]
		public string? OfficialName { get; set; } = string.Empty;

		[JsonPropertyName("persistent_account_id")]
		public string? PersistentAccountId { get; set; } = string.Empty;

		[JsonPropertyName("subtype")]
		public string? Subtype { get; set; } = string.Empty;

		[JsonPropertyName("type")]
		public string? Type { get; set; } = string.Empty;

		[JsonPropertyName("verification_status")]
		public string? VerificationStatus { get; set; } = string.Empty;
	}
	public class Balances
	{
		[JsonPropertyName("available")]
		public decimal? Available { get; set; }

		[JsonPropertyName("current")]
		public decimal? Current { get; set; }

		[JsonPropertyName("limit")]
		public decimal? Limit { get; set; }

		[JsonPropertyName("iso_currency_code")]
		public string? IsoCurrencyCode { get; set; } = string.Empty;

		[JsonPropertyName("unofficial_currency_code")]
		public string? UnofficialCurrencyCode { get; set; }

		[JsonPropertyName("last_updated_datetime")]
		public string? LastUpdateDatetime { get; set; } = string.Empty;
	}
}
