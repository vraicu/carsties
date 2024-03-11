using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace AuctionService.PlaywrightTests.Fixtures;

public class ContextTestWithConfiguration : ContextTest
{
   // public IPage Page { get; private set; }
    public IConfiguration Configuration { get; private set; }

    [SetUp]
    public async Task PageSetup()
    {
        //Page = 
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
}
