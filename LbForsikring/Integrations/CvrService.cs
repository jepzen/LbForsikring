namespace LbForsikring.Integrations
{
    using System;
    using System.Net.Http;

    public class CvrService : ICvrService
    {
        public async Task<string> GetByCvr(string cvr)
        {
            if (string.IsNullOrWhiteSpace(cvr))
                throw new ArgumentException("cvr is required", nameof(cvr));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/{cvr}";

            //TODO: Dont new up the HttpClient
            using var client = new HttpClient();
            // Call the external API synchronously (caller currently expects a non-async API).
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }
            
            //TODO: Map to a model
            return response.Content.ReadAsStringAsync().Result;

        }

        public async Task<string> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is required", nameof(name));

            //TODO: Move to configuration
            var url = $"https://apicvr.dk/api/v1/search/company/{name}";

            using var client = new HttpClient();
            // Call the external API synchronously (caller currently expects a non-async API).
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Request to {url} failed with status {(int)response.StatusCode}.");
            }
          
            //TODO Map to a model
            return  response.Content.ReadAsStringAsync().Result;
        }
    }

    public interface ICvrService
    {
        public Task<string> GetByName(string name);
        public Task<string> GetByCvr(string cvr);
    }
}
