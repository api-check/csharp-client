# ApiCheck C# Client

Address validation, search, and verification for 18 European countries.

## Installation

```bash
dotnet add package ApiCheck.Client
```

## Quick Start

```csharp
using ApiCheck;

var client = new ApiClient("your-api-key");
```

## Global Search (Recommended)

The **global search** endpoint is the most powerful way to find addresses. It searches across streets, cities, and postal codes in one query with powerful filtering options.

```csharp
// Basic search - finds streets, cities, and postal codes
var results = await client.GlobalSearchAsync("nl", query: "Amsterdam", limit: 10);

foreach (var street in results.Results.Streets)
{
    Console.WriteLine($"{street.Name} (street)");
}
foreach (var city in results.Results.Cities)
{
    Console.WriteLine($"{city.Name} (city)");
}
foreach (var pc in results.Results.Postalcodes)
{
    Console.WriteLine($"{pc.Name} (postalcode)");
}

// Filter by city - only return results within a specific city
var cityResults = await client.GlobalSearchAsync("nl", 
    query: "Dam", 
    cityId: 2465, 
    limit: 10);

// Filter by street - only return results on a specific street  
var streetResults = await client.GlobalSearchAsync("nl", 
    query: "1", 
    streetId: 12345, 
    limit: 10);

// Filter by postal code area
var pcResults = await client.GlobalSearchAsync("nl", 
    query: "A", 
    postalcodeId: 54321, 
    limit: 10);

// Belgium: filter by locality (deelgemeente)
var locResults = await client.GlobalSearchAsync("be", 
    query: "Hoofd", 
    localityId: 111, 
    limit: 10);

// Belgium: filter by municipality (gemeente)
var munResults = await client.GlobalSearchAsync("be", 
    query: "Station", 
    municipalityId: 222, 
    limit: 10);

// Combine filters for precise results
var filtered = await client.GlobalSearchAsync("nl", 
    query: "1", 
    cityId: 2465,
    limit: 10);
```

### Global Search Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `country` | string | Country code (nl, be, lu, de, fr, cz, fi, it, no, pl, pt, ro, es, ch, at, dk, gb, se) |
| `query` | string | Search term (street name, city name, or postal code) |
| `limit` | int | Maximum results (default: 10) |
| `cityId` | int? | Filter results to a specific city |
| `streetId` | int? | Filter results to a specific street |
| `postalcodeId` | int? | Filter results to a specific postal code area |
| `localityId` | int? | Filter results to a specific locality (Belgium) |
| `municipalityId` | int? | Filter results to a specific municipality (Belgium) |

### Result Types

Results are grouped in the `Results` object:

- `Streets` - List of street matches
- `Cities` - List of city matches
- `Postalcodes` - List of postal code matches

## Address Lookup (Netherlands & Luxembourg)

For exact address lookup by postal code and house number:

```csharp
// Basic lookup
var address = await client.LookupAsync("nl", "1012LM", "1");
Console.WriteLine(address.Street);  // Damrak
Console.WriteLine(address.City);    // Amsterdam

// With number addition (apartment/suite)
var addressWithAddition = await client.LookupAsync("nl", "1012LM", "1", "A");

// Get available number additions for an address
var additions = await client.GetNumberAdditionsAsync("nl", "1012LM", "1");
Console.WriteLine(string.Join(", ", additions.NumberAdditions));  // A, B, 1-3
```

## Individual Search Endpoints

```csharp
// Search cities
var cities = await client.SearchCityAsync("nl", "Amsterdam", limit: 10);

// Search streets
var streets = await client.SearchStreetAsync("nl", "Damrak", limit: 10);
var streetsInCity = await client.SearchStreetAsync("nl", "Dam", cityId: 2465, limit: 10);

// Search postal codes
var postalcodes = await client.SearchPostalcodeAsync("nl", "1012", limit: 10);

// Search localities (Belgium primarily)
var localities = await client.SearchLocalityAsync("be", "Antwerpen", limit: 10);

// Search municipalities (Belgium primarily)
var municipalities = await client.SearchMunicipalityAsync("be", "Antwerpen", limit: 10);

// Resolve full address using IDs from other searches
var addresses = await client.SearchAddressAsync("nl",
    cityId: 2465,
    number: "1",
    numberAddition: "A",
    limit: 10);
```

## Verification

```csharp
// Verify email
var emailResult = await client.VerifyEmailAsync("test@example.com");
Console.WriteLine(emailResult.Status);          // Valid, Invalid, or Unknown
Console.WriteLine(emailResult.DisposableEmail); // true if disposable
Console.WriteLine(emailResult.Greylisted);      // true if greylisted

// Verify phone number
var phoneResult = await client.VerifyPhoneAsync("+31612345678");
Console.WriteLine(phoneResult.Valid);           // true if valid
Console.WriteLine(phoneResult.CountryCode);     // NL
```

## Supported Countries

### All Search Endpoints (18 countries)
`nl`, `be`, `lu`, `de`, `fr`, `cz`, `fi`, `it`, `no`, `pl`, `pt`, `ro`, `es`, `ch`, `at`, `dk`, `gb`, `se`

### Address Lookup (Netherlands & Luxembourg only)
`nl`, `lu`

## API Key

Get your API key at [app.apicheck.nl](https://app.apicheck.nl)

## Options

```csharp
var client = new ApiClient("your-api-key", referer: "https://yoursite.com");
```

The `referer` parameter is required if your API key has "Allowed Hosts" enabled.

## Tips

1. **Use Global Search first** - It's the most flexible and covers all use cases
2. **Filter for precision** - Use cityId, streetId, etc. to narrow down results
3. **Chain searches** - Use Search City to get a cityId, then use it in Global Search or Search Address
4. **Belgium addresses** - Use localityId and municipalityId filters for precise results

## License

MIT
