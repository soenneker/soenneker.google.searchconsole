using Soenneker.Google.SearchConsole.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Google.SearchConsole.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class GoogleSearchConsoleUtilTests : HostedUnitTest
{
    private readonly IGoogleSearchConsoleUtil _util;

    public GoogleSearchConsoleUtilTests(Host host) : base(host)
    {
        _util = Resolve<IGoogleSearchConsoleUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
