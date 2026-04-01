using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ApiCheck;

public static class Countries
{
    public static readonly string[] All = { "nl", "be", "lu", "de", "fr", "cz", "fi", "it", "no", "pl", "pt", "ro", "es", "ch", "at", "dk", "gb", "se" };
    public static readonly string[] Lookup = { "nl", "lu" };
}

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

    // ============================================
    // Lookup API (Netherlands, Luxembourg only)
    // ============================================

    public async Task<T?> LookupAsync<T>(string country, string postalcode, string number, string? numberAddition = null)
    {
        var url = $"/lookup/v1/postalcode/{country.ToLower()}?postalcode={Uri.EscapeDataString(postalcode)}&number={Uri.EscapeDataString(number)}";
        if (numberAddition != null)
            url += $"&numberAddition={Uri.EscapeDataString(numberAddition)}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> GetNumberAdditionsAsync<T>(string country, string postalcode, string number)
    {
        var response = await _httpClient.GetAsync(
            $"/lookup/v1/address/{country.ToLower()}?postalcode={Uri.EscapeDataString(postalcode)}&number={Uri.EscapeDataString(number)}&fields=[\"numberAdditions\"]");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    // ============================================
    // Search API (18 European countries)
    // ============================================

    public async Task<T?> GlobalSearchAsync<T>(string country, string query, 
        int limit = 10, 
        int? cityId = null, 
        int? streetId = null, 
        int? postalcodeId = null,
        int? localityId = null,
        int? municipalityId = null)
    {
        var url = $"/search/v1/global/{country.ToLower()}?query={Uri.EscapeDataString(query)}&limit={limit}";
        if (cityId.HasValue) url += $"&city_id={cityId}";
        if (streetId.HasValue) url += $"&street_id={streetId}";
        if (postalcodeId.HasValue) url += $"&postalcode_id={postalcodeId}";
        if (localityId.HasValue) url += $"&locality_id={localityId}";
        if (municipalityId.HasValue) url += $"&municipality_id={municipalityId}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchCityAsync<T>(string country, string name, int limit = 10, int? cityId = null)
    {
        var url = $"/search/v1/city/{country.ToLower()}?name={Uri.EscapeDataString(name)}&limit={limit}";
        if (cityId.HasValue) url += $"&city_id={cityId}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchStreetAsync<T>(string country, string name, int limit = 10, int? cityId = null)
    {
        var url = $"/search/v1/street/{country.ToLower()}?name={Uri.EscapeDataString(name)}&limit={limit}";
        if (cityId.HasValue) url += $"&city_id={cityId}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchPostalcodeAsync<T>(string country, string name, int limit = 10, int? cityId = null)
    {
        var url = $"/search/v1/postalcode/{country.ToLower()}?name={Uri.EscapeDataString(name)}&limit={limit}";
        if (cityId.HasValue) url += $"&city_id={cityId}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchLocalityAsync<T>(string country, string name, int limit = 10)
    {
        var response = await _httpClient.GetAsync(
            $"/search/v1/locality/{country.ToLower()}?name={Uri.EscapeDataString(name)}&limit={limit}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchMunicipalityAsync<T>(string country, string name, int limit = 10)
    {
        var response = await _httpClient.GetAsync(
            $"/search/v1/municipality/{country.ToLower()}?name={Uri.EscapeDataString(name)}&limit={limit}");
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    public async Task<T?> SearchAddressAsync<T>(string country,
        int? streetId = null,
        int? cityId = null,
        int? postalcodeId = null,
        int? localityId = null,
        int? municipalityId = null,
        string? number = null,
        string? numberAddition = null,
        int limit = 10)
    {
        var parts = new List<string>();
        if (streetId.HasValue) parts.Add($"street_id={streetId}");
        if (cityId.HasValue) parts.Add($"city_id={cityId}");
        if (postalcodeId.HasValue) parts.Add($"postalcode_id={postalcodeId}");
        if (localityId.HasValue) parts.Add($"locality_id={localityId}");
        if (municipalityId.HasValue) parts.Add($"municipality_id={municipalityId}");
        if (number != null) parts.Add($"number={Uri.EscapeDataString(number)}");
        if (numberAddition != null) parts.Add($"numberAddition={Uri.EscapeDataString(numberAddition)}");
        parts.Add($"limit={limit}");
        
        var url = $"/search/v1/address/{country.ToLower()}?" + string.Join("&", parts);
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return wrapper.Data;
    }

    // ============================================
    // Verify API
    // ============================================

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
        [JsonPropertyName("error")]
        public bool Error { get; set; }
        
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    }
}
