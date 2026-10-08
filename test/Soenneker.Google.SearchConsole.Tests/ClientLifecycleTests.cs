using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1;
using Soenneker.Google.Credentials.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Google.SearchConsole.Tests;

public sealed class ClientLifecycleTests
{
    [Test]
    public async ValueTask Clients_are_cached_per_file_and_recreated_after_removal(CancellationToken cancellationToken)
    {
        var credentials = new FakeCredentials();
        await using var util = new GoogleSearchConsoleUtil(credentials);
        SearchConsoleService first = await util.Get("first.json", cancellationToken: cancellationToken);
        SearchConsoleService cached = await util.Get("first.json", cancellationToken: cancellationToken);
        SearchConsoleService second = await util.Get("second.json", cancellationToken: cancellationToken);
        if (!ReferenceEquals(first, cached) || ReferenceEquals(first, second) || credentials.Calls != 2)
            throw new InvalidOperationException("Clients must be cached independently by credential filename.");
        if (credentials.Scope != SearchConsoleService.Scope.Webmasters)
            throw new InvalidOperationException("Search Console requires the webmasters scope.");
        if (!await util.Remove("first.json", cancellationToken: cancellationToken))
            throw new InvalidOperationException("Removal should report an existing client.");
        SearchConsoleService recreated = await util.Get("first.json", cancellationToken: cancellationToken);
        if (ReferenceEquals(first, recreated) || credentials.Calls != 3)
            throw new InvalidOperationException("Removal must allow a new client to be created.");
    }

    [Test]
    public async ValueTask Invalid_property_is_rejected_before_loading_credentials(CancellationToken cancellationToken)
    {
        var credentials = new FakeCredentials();
        await using var util = new GoogleSearchConsoleUtil(credentials);
        try
        {
            await util.GetSite(" ", "account.json", cancellationToken: cancellationToken);
        }
        catch (ArgumentException)
        {
            if (credentials.Calls != 0)
                throw new InvalidOperationException("Invalid requests must not load credentials.");
            return;
        }
        throw new InvalidOperationException("Expected an argument exception.");
    }

    private sealed class FakeCredentials : IGoogleCredentialsUtil
    {
        public int Calls { get; private set; }
        public string? Scope { get; private set; }

        public ValueTask<ICredential> Get(string fileName, string[] scopes, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Calls++;
            Scope = scopes[0];
            return ValueTask.FromResult<ICredential>(GoogleCredential.FromAccessToken("unused-test-token"));
        }

        public ValueTask<bool> Remove(string fileName, string[] scopes, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
        public void RemoveSync(string fileName, string[] scopes, CancellationToken cancellationToken = default) { }
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
