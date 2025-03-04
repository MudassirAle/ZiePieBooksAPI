using Going.Plaid.Entity;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core.Model.Plaid
{
    public record NormalizedTransaction
    {
        [JsonPropertyName("account_id")]
        public string? AccountId { get; init; } = default!;

        [JsonPropertyName("amount")]
        public decimal? Amount { get; init; } = default!;

        [JsonPropertyName("iso_currency_code")]
        public string? IsoCurrencyCode { get; init; } = default!;

        [JsonPropertyName("unofficial_currency_code")]
        public string? UnofficialCurrencyCode { get; init; } = default!;

        [JsonPropertyName("category")]
        [Obsolete]
        public IReadOnlyList<NormalizedCategory>? Category { get; init; } = default!;

        [JsonPropertyName("category_id")]
        [Obsolete]
        public string? CategoryId { get; init; } = default!;

        [JsonPropertyName("check_number")]
        public string? CheckNumber { get; init; } = default!;

        [JsonPropertyName("date")]
        public DateOnly? Date { get; init; } = default!;

        [JsonPropertyName("location")]
        public Location? Location { get; init; } = default!;

        [JsonPropertyName("name")]
        public string? Name { get; init; } = default!;

        [JsonPropertyName("merchant_name")]
        public string? MerchantName { get; init; } = default!;

        [JsonPropertyName("original_description")]
        public string? OriginalDescription { get; init; } = default!;

        [JsonPropertyName("payment_meta")]
        public Going.Plaid.Entity.PaymentMeta? PaymentMeta { get; init; } = default!;

        [JsonPropertyName("pending")]
        public bool? Pending { get; init; } = default!;

        [JsonPropertyName("pending_transaction_id")]
        public string? PendingTransactionId { get; init; } = default!;

        [JsonPropertyName("account_owner")]
        public string? AccountOwner { get; init; } = default!;

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; init; } = default!;

        [JsonPropertyName("transaction_type")]
        [Obsolete]
        public Going.Plaid.Entity.TransactionTransactionTypeEnum? TransactionType { get; init; } = default!;

        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; init; } = default!;

        [JsonPropertyName("website")]
        public string? Website { get; init; } = default!;

        [JsonPropertyName("authorized_date")]
        public DateOnly? AuthorizedDate { get; init; } = default!;

        [JsonPropertyName("authorized_datetime")]
        public DateTimeOffset? AuthorizedDatetime { get; init; } = default!;

        [JsonPropertyName("datetime")]
        public DateTimeOffset? Datetime { get; init; } = default!;

        [JsonPropertyName("payment_channel")]
        public Going.Plaid.Entity.TransactionPaymentChannelEnum? PaymentChannel { get; init; } = default!;

        [JsonPropertyName("personal_finance_category")]
        public PersonalFinanceCategory? PersonalFinanceCategory { get; init; } = default!;

        [JsonPropertyName("transaction_code")]
        public TransactionCode? TransactionCode { get; init; } = default!;

        [JsonPropertyName("personal_finance_category_icon_url")]
        public string? PersonalFinanceCategoryIconUrl { get; init; } = default!;

        [JsonPropertyName("counterparties")]
        public IReadOnlyList<Counterparty>? Counterparties { get; init; } = default!;

        [JsonPropertyName("merchant_entity_id")]
        public string? MerchantEntityId { get; init; } = default!;
    }
    public class NormalizedCategory
    {
        public string? Name { get; set; }
    }

    public class PlaidTransaction
    {
        public int ID { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public int TotalTransactions { get; set; }
        public DateTime LastSync { get; set; }

        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class PlaidTransactionModel
    {
        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }

        [JsonPropertyName("account_owner")]
        public string? AccountOwner { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("iso_currency_code")]
        public string? IsoCurrencyCode { get; set; }

        [JsonPropertyName("unofficial_currency_code")]
        public string? UnofficialCurrencyCode { get; set; }

        [JsonPropertyName("category")]
        public List<string>? Category { get; set; }

        [JsonPropertyName("category_id")]
        public string? CategoryId { get; set; }

        [JsonPropertyName("check_number")]
        public string? CheckNumber { get; set; }

        [JsonPropertyName("counterparties")]
        public List<Counterparty> Counterparties { get; set; } = new List<Counterparty> { };

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("datetime")]
        public string? Datetime { get; set; } //( YYYY-MM-DDTHH:mm:ssZ )

        [JsonPropertyName("authorized_date")]
        public string? AuthorizedDate { get; set; }

        [JsonPropertyName("authorized_datetime")]
        public string? AuthorizedDatetime { get; set; } //( YYYY-MM-DDTHH:mm:ssZ )

        [JsonPropertyName("location")]
        public Location Location { get; set; } = new Location();

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("merchant_name")]
        public string? MerchantName { get; set; }

        [JsonPropertyName("merchant_entity_id")]
        public string? MerchantEntityId { get; set; }

        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("payment_meta")]
        public PaymentMeta PaymentMeta { get; set; } = new PaymentMeta();

        [JsonPropertyName("payment_channel")]
        public string? PaymentChannel { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("pending_transaction_id")]
        public string? PendingTransactionId { get; set; }

        [JsonPropertyName("personal_finance_category")]
        public PersonalFinanceCategory PersonalFinanceCategory { get; set; } = new PersonalFinanceCategory();

        [JsonPropertyName("personal_finance_category_icon_url")]
        public string? PersonalFinanceCategoryIconUrl { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("transaction_code")]
        public TransactionCode? TransactionCode { get; set; }

        [JsonPropertyName("transaction_type")]
        public string? TransactionType { get; set; }
    }

    public class Counterparty
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("entity_id")]
        public string? EntityId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("logo_url")]
        public string? LogoUrl { get; set; }

        [JsonPropertyName("confidence_level")]
        public string? ConfidenceLevel { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        [JsonPropertyName("store_number")]
        public string? StoreNumber { get; set; }
    }

    public class PaymentMeta
    {
        [JsonPropertyName("reference_number")]
        public string? ReferenceNumber { get; set; }

        [JsonPropertyName("ppd_id")]
        public string? PpdId { get; set; }

        [JsonPropertyName("payee")]
        public string? Payee { get; set; }

        [JsonPropertyName("by_order_of")]
        public string? ByOrderOf { get; set; }

        [JsonPropertyName("payer")]
        public string? Payer { get; set; }

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_processor")]
        public string? PaymentProcessor { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

    public class PersonalFinanceCategory
    {
        [JsonPropertyName("primary")]
        public string? Primary { get; set; }

        [JsonPropertyName("detailed")]
        public string? Detailed { get; set; }

        [JsonPropertyName("confidence_level")]
        public string? ConfidenceLevel { get; set; }
    }

    public enum TransactionCode
    {
        [EnumMember(Value = "adjustment")]
        Adjustment,
        [EnumMember(Value = "atm")]
        Atm,
        [EnumMember(Value = "bank charge")]
        BankCharge,
        [EnumMember(Value = "bill payment")]
        BillPayment,
        [EnumMember(Value = "cash")]
        Cash,
        [EnumMember(Value = "cashback")]
        Cashback,
        [EnumMember(Value = "cheque")]
        Cheque,
        [EnumMember(Value = "direct debit")]
        DirectDebit,
        [EnumMember(Value = "interest")]
        Interest,
        [EnumMember(Value = "purchase")]
        Purchase,
        [EnumMember(Value = "standing order")]
        StandingOrder,
        [EnumMember(Value = "transfer")]
        Transfer,
        [EnumMember(Value = "undefined")]
        Undefined,
    }
}
