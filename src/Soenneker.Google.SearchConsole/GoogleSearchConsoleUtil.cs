using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;
using Google.Apis.Services;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Dictionaries.SingletonKeys;
using Soenneker.Google.Credentials.Abstract;
using Soenneker.Google.SearchConsole.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Google.SearchConsole;

public sealed class GoogleSearchConsoleUtil : IGoogleSearchConsoleUtil
{
    private static readonly string[] _scopes = [SearchConsoleService.Scope.Webmasters];
    private readonly IGoogleCredentialsUtil _credentials;
    private readonly SingletonKeyDictionary<string, SearchConsoleService> _services;

    public GoogleSearchConsoleUtil(IGoogleCredentialsUtil credentials)
    {
        _credentials = credentials;
        _services = new SingletonKeyDictionary<string, SearchConsoleService>(CreateService);
    }

    private async ValueTask<SearchConsoleService> CreateService(string fileName, CancellationToken cancellationToken)
    {
        var credential = await _credentials.Get(fileName, _scopes, cancellationToken).NoSync();
        return new SearchConsoleService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Soenneker.Google.SearchConsole"
        });
    }

    public ValueTask<SearchConsoleService> Get(string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return _services.Get(fileName, cancellationToken);
    }

    public async ValueTask<SearchAnalyticsQueryResponse> QuerySearchAnalytics(string siteUrl,
        SearchAnalyticsQueryRequest request, string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        ArgumentNullException.ThrowIfNull(request);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        return await service.Searchanalytics.Query(request, siteUrl).ExecuteAsync(cancellationToken)
                            .NoSync();
    }

    public async ValueTask<InspectUrlIndexResponse> InspectUrl(string siteUrl, string inspectionUrl, string fileName,
        string? languageCode = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(inspectionUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        return await service.UrlInspection.Index
                            .Inspect(new InspectUrlIndexRequest
                                { SiteUrl = siteUrl, InspectionUrl = inspectionUrl, LanguageCode = languageCode })
                            .ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask<SitesListResponse> ListSites(string fileName, CancellationToken cancellationToken = default)
    {
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        return await service.Sites.List().ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask<WmxSite> GetSite(string siteUrl, string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        return await service.Sites.Get(siteUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask AddSite(string siteUrl, string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        await service.Sites.Add(siteUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask DeleteSite(string siteUrl, string fileName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        await service.Sites.Delete(siteUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask<SitemapsListResponse> ListSitemaps(string siteUrl, string fileName,
        string? sitemapIndex = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        var request = service.Sitemaps.List(siteUrl);
        request.SitemapIndex = sitemapIndex;
        return await request.ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask<WmxSitemap> GetSitemap(string siteUrl, string sitemapUrl, string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitemapUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        return await service.Sitemaps.Get(siteUrl, sitemapUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask SubmitSitemap(string siteUrl, string sitemapUrl, string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitemapUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        await service.Sitemaps.Submit(siteUrl, sitemapUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public async ValueTask DeleteSitemap(string siteUrl, string sitemapUrl, string fileName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(siteUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(sitemapUrl);
        SearchConsoleService service = await Get(fileName, cancellationToken).NoSync();
        await service.Sitemaps.Delete(siteUrl, sitemapUrl).ExecuteAsync(cancellationToken).NoSync();
    }

    public ValueTask<bool> Remove(string fileName, CancellationToken cancellationToken = default) =>
        _services.Remove(fileName, cancellationToken);

    public void RemoveSync(string fileName, CancellationToken cancellationToken = default) =>
        _services.RemoveSync(fileName, cancellationToken);

    public void Dispose() => _services.Dispose();

    public ValueTask DisposeAsync() => _services.DisposeAsync();
}