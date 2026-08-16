namespace LbForsikring.Integrations
{
    using System;
    using System.Net.Http;
    using System.Text.Json;

    public class CvrService : ICvrService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly HttpClient Client = new();

        public async Task<CvrResponse> GetByCvr(string cvr)
        {
            if (string.IsNullOrWhiteSpace(cvr))
                throw new ArgumentException("cvr is required", nameof(cvr));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/{cvr}";
            
            var response = await Client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var cvrResponse = JsonSerializer.Deserialize<CvrResponse>(content, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize CVR response");

            return cvrResponse;
        }

        public async Task<CvrResponse> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is required", nameof(name));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/search/company/{name}";

            var response = await Client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var cvrResponses = JsonSerializer.Deserialize<List<CvrResponse>>(content, JsonOptions)
                               ?? throw new InvalidOperationException("Failed to deserialize CVR response");

            if (cvrResponses.Count == 0)
                throw new InvalidOperationException($"No companies found matching name '{name}'");

            return cvrResponses[0];
        }
    }

    public interface ICvrService
    {
        public Task<CvrResponse> GetByCvr(string cvr);
        public Task<CvrResponse> GetByName(string name);
    }
}

