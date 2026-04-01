using System.Net.Http.Json;

namespace ApiCheck;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly string BaseUrl = "https://api.apicheck.nl";

    public ApiClient(string apiKey, string? referer = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        if (referer != null) _httpClient.DefaultRequestHeaders.Add("Referer", referer);
    }

    // Lookup API
    public async Task<T?> LookupAsync<T>(string country, string postalcode, string number)
    {
        var response = await _httpClient.GetAsync(
            $"/lookup/v1/postalcode/{country.ToLower()}?postalcode={postalcode}&number={number}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> GetNumberAdditionsAsync<T>(string country, string postalcode, string number)
    {
        var response = await _httpClient.GetAsync(
            $"/lookup/v1/address/{country.ToLower()}?postalcode={postalcode}&number={number}&fields=[\"numberAdditions\"]");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    // Search API
    public async Task<T?> GlobalSearchAsync<T>(string country, string query, int limit = 10)
    {
        var response = await _httpClient.GetAsync(
            $"/search/v1/global/{country.ToLower()}?query={Uri.EscapeDataString(query)}&limit={limit}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    // Verify API
    public async Task<T?> VerifyEmailAsync<T>(string email)
    {
        var response = await _httpClient.GetAsync($"/verify/v1/email/?email={Uri.EscapeDataString(email)}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> VerifyPhoneAsync<T>(string number)
    {
        var response = await _httpClient.GetAsync($"/verify/v1/phone/?number={Uri.EscapeDataString(number)}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    private class ApiResponse<T>
    {
        public bool Error { get; set; }
        public T Data { get; set; } = default!;
    }
}
