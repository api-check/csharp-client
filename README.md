# ApiCheck C# Client

Address validation, search, and verification for 18 European countries.

## Installation
dotnet add package ApiCheck.Client

## Quick Start
using ApiCheck;

var client = new ApiClient("your-api-key");

## Global Search (Recommended)
The **global search** endpoint is the most powerful way to find addresses. It searches across streets, cities, and postal codes in one query with powerful filtering options.
