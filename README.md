[![](https://img.shields.io/nuget/v/soenneker.google.searchconsole.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.google.searchconsole/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.google.searchconsole/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.google.searchconsole/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.google.searchconsole.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.google.searchconsole/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Google.SearchConsole
### A utility library for Google Search Console.

## Installation

```
dotnet add package Soenneker.Google.SearchConsole
```

## Setup

Enable the Google Search Console API in your Google Cloud project and grant the service account access to your Search Console property. Place its credential JSON in the application's output directory under `LocalResources/search-console.json`. Do not commit this file.

Register the utility and its credential provider:

```csharp
using Soenneker.Google.SearchConsole.Registrars;

services.AddGoogleSearchConsoleUtilAsSingleton();
// Or: services.AddGoogleSearchConsoleUtilAsScoped();
```

Inject `IGoogleSearchConsoleUtil`. Clients are cached by credential filename and request the `https://www.googleapis.com/auth/webmasters` scope, which supports both read and write operations.

## Usage

```csharp
using Google.Apis.SearchConsole.v1.Data;
using Soenneker.Google.SearchConsole.Abstract;

public sealed class SearchReport(IGoogleSearchConsoleUtil searchConsole)
{
    public async Task<SearchAnalyticsQueryResponse> Get(CancellationToken cancellationToken)
    {
        return await searchConsole.QuerySearchAnalytics(
            "sc-domain:example.com",
            new SearchAnalyticsQueryRequest
            {
                StartDate = "2026-08-01",
                EndDate = "2026-08-31",
                Dimensions = ["query", "page"],
                RowLimit = 25000,
                StartRow = 0
            },
            "search-console.json",
            cancellationToken);
    }
}
```

Use the exact property identifier from Search Console: `sc-domain:example.com` for a domain property or `https://example.com/` for a URL-prefix property.

Available operations:

- `QuerySearchAnalytics`: returns one page of analytics using Google's request and response models. Set `StartRow` and `RowLimit` explicitly for pagination. Search Console may return only the top rows, even with pagination.
- `InspectUrl`: retrieves Google's indexed status of a URL; it does not run a live test or request indexing.
- `ListSites`, `GetSite`, `AddSite`, `DeleteSite`: manage account properties. Adding a property does not verify ownership.
- `ListSitemaps`, `GetSitemap`, `SubmitSitemap`, `DeleteSitemap`: manage sitemap submissions. `ListSitemaps` accepts an optional sitemap index.
- `Get`: exposes the cached SDK client for advanced usage. Do not dispose this client directly.
- `Remove` and `RemoveSync`: evict and dispose a cached client. The separate credential cache is unchanged; evict credentials through `IGoogleCredentialsUtil` as well when rotating credential files.

All asynchronous operations accept cancellation tokens. Google API errors propagate to the caller. DI disposes cached clients with the utility; manually constructed utilities should be disposed. Do not remove or dispose clients while requests are in flight.

[Google Search Console API reference](https://developers.google.com/webmaster-tools/v1/api_reference_index)
