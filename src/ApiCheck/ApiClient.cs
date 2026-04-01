using System.Net.Http.Json;
using System.Text.Json;

namespace ApiCheck;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    private static readonly HashSet<string> LookupCountries = new() { "nl", "lu" };
    private static readonly HashSet<string> SearchCountries = new() 
    { "nl", "be", "lu", "fr", "de", "cz", "fi", "it", "no", "pl", "pt", "ro", "es", "ch", "at", "dk", "gb", "se" };

    public ApiClient(string apiKey, string? referer = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.apicheck.nl"),
            Timeout = TimeSpan.FromSeconds(10)
        };
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        if (referer != null) _httpClient.DefaultRequestHeaders.Add("Referer", referer);
    }

    public async Task<LookupResponse> LookupAsync(string country, string postalCode, string number)
    {
        var c = country.ToLower();
        if (!LookupCountries.Contains(c)) throw new UnsupportedCountryException($"Country '{country}' not supported", country);
        var response = await _httpClient.GetAsync($"/lookup/v1/address/?country={c}&postalcode={postalCode}&number={number}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LookupResponse>() ?? throw new ApiCheckException("Empty response");
    }

    public async Task<GlobalSearchResponse> GlobalSearchAsync(string country, string query, int limit = 10)
    {
        var c = country.ToLower();
        if (!SearchCountries.Contains(c)) throw new UnsupportedCountryException($"Country '{country}' not supported", country);
        var response = await _httpClient.GetAsync($"/search/v1/global/?country={c}&query={Uri.EscapeDataString(query)}&limit={limit}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GlobalSearchResponse>() ?? throw new ApiCheckException("Empty response");
    }

    public async Task<EmailVerificationResponse> VerifyEmailAsync(string email)
    {
        var response = await _httpClient.GetAsync($"/verify/v1/email/?email={Uri.EscapeDataString(email)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmailVerificationResponse>() ?? throw new ApiCheckException("Empty response");
    }

    public async Task<PhoneVerificationResponse> VerifyPhoneAsync(string number)
    {
        var response = await _httpClient.GetAsync($"/verify/v1/phone/?number={Uri.EscapeDataString(number)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PhoneVerificationResponse>() ?? throw new ApiCheckException("Empty response");
    }
}
