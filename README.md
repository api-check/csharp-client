# ApiCheck C# Client

C#/.NET client for ApiCheck - address validation, search, and verification.

## Installation

```bash
dotnet add package ApiCheck.Client
```

## Usage

```csharp
using ApiCheck;

var client = new ApiClient("your-api-key");

// Lookup address (NL, LU)
var address = await client.LookupAsync("nl", "1012LM", "1");
Console.WriteLine($"{address.Street} {address.Number}, {address.City}");

// Global search (18 countries)
var results = await client.GlobalSearchAsync("nl", "Amsterdam");

// Verify email
var email = await client.VerifyEmailAsync("test@example.com");
Console.WriteLine(email.Status); // valid, invalid, unknown

// Verify phone
var phone = await client.VerifyPhoneAsync("+31612345678");
Console.WriteLine(phone.Valid); // true/false
```

## API

### `LookupAsync(country, postalCode, number)`
Address lookup for NL and LU.

### `GlobalSearchAsync(country, query, limit)`
Search across 18 countries.

### `VerifyEmailAsync(email)`
Verify email address.

### `VerifyPhoneAsync(number)`
Verify phone number.

## Requirements

- .NET 8.0+

## License

MIT

## Support

- Website: [apicheck.nl](https://apicheck.nl)
- Email: support@apicheck.nl
