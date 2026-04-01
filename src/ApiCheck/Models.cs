namespace ApiCheck;

public record LookupResponse(
    string Street,
    string Number,
    string PostalCode,
    string City,
    Country Country,
    Coordinates Coordinates,
    string? StreetShort = null,
    string? NumberAddition = null,
    string? Municipality = null
);

public record Country(string Name, string Code);
public record Coordinates(double Latitude, double Longitude);

public record NumberAdditionsResponse(string Number, string[] NumberAdditions);

public record GlobalSearchResponse(SearchResult[] Results, int Count);

public record SearchResult(
    int Id,
    string Name,
    string? Type = null,
    double? Latitude = null,
    double? Longitude = null
);

public record EmailVerificationResponse(
    string Email,
    string Status,
    [property: JsonPropertyName("disposable_email")] bool DisposableEmail,
    bool Greylisted
);

public record PhoneVerificationResponse(
    string Number,
    bool Valid,
    string? CountryCode = null,
    string? Carrier = null
);

public class ApiCheckException : Exception
{
    public int? StatusCode { get; }
    public ApiCheckException(string message, int? statusCode = null) : base(message) => StatusCode = statusCode;
}

public class UnsupportedCountryException : ApiCheckException
{
    public string Country { get; }
    public UnsupportedCountryException(string message, string country) : base(message, 400) => Country = country;
}

public class AuthenticationException : ApiCheckException
{
    public AuthenticationException() : base("Invalid API key", 401) { }
}

public class RateLimitException : ApiCheckException
{
    public int? RetryAfter { get; }
    public RateLimitException(int? retryAfter) : base("Rate limit exceeded", 429) => RetryAfter = retryAfter;
}
