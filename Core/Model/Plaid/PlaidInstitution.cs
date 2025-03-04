using System.Text.Json.Serialization;

namespace Core.Model.Plaid
{
    public class PlaidInstitutionDTO
    {
        public int ID { get; set; }
        public List<PlaidInstitution> PlaidInstitutions { get; set; } = new List<PlaidInstitution>();
    }
    public class PlaidInstitution
    {
        [JsonPropertyName("country_codes")]
        public List<string> CountryCodes { get; set; } = new List<string>();

        [JsonPropertyName("dtc_numbers")]
        public List<string> DtcNumbers { get; set; } = new List<string>();

        [JsonPropertyName("institution_id")]
        public string InstitutionId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("oauth")]
        public bool Oauth { get; set; } = false;

        [JsonPropertyName("products")]
        public List<string> Products { get; set; } = new List<string>();

        [JsonPropertyName("routing_numbers")]
        public List<string> RoutingNumbers { get; set; } = new List<string>();
    }
}
