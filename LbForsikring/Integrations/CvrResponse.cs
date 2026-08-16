namespace LbForsikring.Integrations
{
    using System.Text.Json.Serialization;

    public class CvrResponse
    {
        [JsonPropertyName("vat")]
        public long Vat { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("zipcode")]
        public int Zipcode { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("cityname")]
        public string? CityName { get; set; }

        [JsonPropertyName("protected")]
        public bool Protected { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("startdate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("enddate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("employees")]
        public int? Employees { get; set; }

        [JsonPropertyName("industrycode")]
        public string IndustryCode { get; set; } = string.Empty;

        [JsonPropertyName("industrydesc")]
        public string IndustryDesc { get; set; } = string.Empty;

        [JsonPropertyName("companycode")]
        public int CompanyCode { get; set; }

        [JsonPropertyName("companydesc")]
        public string CompanyDesc { get; set; } = string.Empty;

        [JsonPropertyName("bankrupt")]
        public bool Bankrupt { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("companytypeshort")]
        public string CompanyTypeShort { get; set; } = string.Empty;

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}
