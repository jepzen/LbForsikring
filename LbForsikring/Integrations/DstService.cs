namespace LbForsikring.Integrations
{
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;

    public class DstService(HttpClient httpClient) : IDstService
    {
        private const string DstApiUrl = "https://api.statbank.dk/v1/data";

        public async Task<string> GetIndustryStats(string industryCode, int year)
        {
            var payload = new DstDataRequest
            {
                Table = "ERHV1",
                Format = "CSV",
                Variables = new[]
                {
                    new DstVariable
                    {
                        Code = "BRANCHE07",
                        Values = new[] { industryCode }
                    },
                    new DstVariable
                    {
                        Code = "TAL",
                        Values = new[] { "ARBSTED", "ANSATTE", "FULDBESK", "LØNSUM" }
                    },
                    new DstVariable
                    {
                        Code = "Tid",
                        Values = new[] { year.ToString() }
                    }
                }
            };

            var response = await httpClient.PostAsJsonAsync(DstApiUrl, payload);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    public interface IDstService
    {
        public Task<string> GetIndustryStats(string industryCode, int year);
    }

    public class DstDataRequest
    {
        [JsonPropertyName("table")]
        public required string Table { get; set; }

        [JsonPropertyName("format")]
        public required string Format { get; set; }

        [JsonPropertyName("variables")]
        public required DstVariable[] Variables { get; set; }
    }

    public class DstVariable
    {
        [JsonPropertyName("code")]
        public required string Code { get; set; }

        [JsonPropertyName("values")]
        public required string[] Values { get; set; }
    }
}
