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

        public async Task<CvrResponse> GetByCvr(string cvr)
        {
            if (string.IsNullOrWhiteSpace(cvr))
                throw new ArgumentException("cvr is required", nameof(cvr));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/{cvr}";

            //TODO: Dont new up the HttpClient
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var cvrResponse = JsonSerializer.Deserialize<CvrResponse>(content, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize CVR response");

            return cvrResponse;
        }

        public async Task<string> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is required", nameof(name));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/search/company/{name}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }

            return await response.Content.ReadAsStringAsync();
        }
    }

    public interface ICvrService
    {
        public Task<CvrResponse> GetByCvr(string cvr);
        public Task<string> GetByName(string name);
    }
}

