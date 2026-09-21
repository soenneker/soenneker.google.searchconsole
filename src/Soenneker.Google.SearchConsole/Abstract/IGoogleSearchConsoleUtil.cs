using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Google.SearchConsole.Abstract;

/// <summary>Provides Search Console operations using cached service-account clients.</summary>
/// <remarks>
/// Credential filenames are relative to LocalResources. Credentials require the webmasters scope and access to the target property.
/// Property identifiers must exactly match Search Console: sc-domain:example.com or a URL prefix such as https://example.com/.
/// API errors and cancellation propagate to the caller.
/// </remarks>
public interface IGoogleSearchConsoleUtil : IDisposable, IAsyncDisposable
{
    /// <summary>Gets a cached SDK client. The utility owns its lifetime; callers must not dispose it.</summary>
    ValueTask<SearchConsoleService> Get(string fileName, CancellationToken cancellationToken = default);

    /// <summary>Queries one page of search analytics. Set StartRow and RowLimit on the request for pagination; Google may omit rows due to API limits.</summary>
    ValueTask<SearchAnalyticsQueryResponse> QuerySearchAnalytics(string siteUrl, SearchAnalyticsQueryRequest request, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Inspects Google's indexed version of a URL, rather than running a live test or requesting indexing.</summary>
    ValueTask<InspectUrlIndexResponse> InspectUrl(string siteUrl, string inspectionUrl, string fileName, string? languageCode = null, CancellationToken cancellationToken = default);

    /// <summary>Lists properties accessible to the credential.</summary>
    ValueTask<SitesListResponse> ListSites(string fileName, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a property's permission information.</summary>
    ValueTask<WmxSite> GetSite(string siteUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Adds a property to the account; this does not verify ownership.</summary>
    ValueTask AddSite(string siteUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Removes a property from the account.</summary>
    ValueTask DeleteSite(string siteUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Lists submitted sitemaps, optionally limited to a sitemap index.</summary>
    ValueTask<SitemapsListResponse> ListSitemaps(string siteUrl, string fileName, string? sitemapIndex = null, CancellationToken cancellationToken = default);

    /// <summary>Retrieves information about a submitted sitemap.</summary>
    ValueTask<WmxSitemap> GetSitemap(string siteUrl, string sitemapUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Submits a sitemap for a property.</summary>
    ValueTask SubmitSitemap(string siteUrl, string sitemapUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Deletes a sitemap submission from a property.</summary>
    ValueTask DeleteSitemap(string siteUrl, string sitemapUrl, string fileName, CancellationToken cancellationToken = default);

    /// <summary>Removes and disposes a cached client. Does not evict credentials. Do not remove clients while requests are in flight.</summary>
    ValueTask<bool> Remove(string fileName, CancellationToken cancellationToken = default);

    /// <summary>Synchronously removes and disposes a cached client. Does not evict credentials. Do not remove clients while requests are in flight.</summary>
    void RemoveSync(string fileName, CancellationToken cancellationToken = default);
}
