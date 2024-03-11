using AuctionService.PlaywrightTests.Fixtures;
using AuctionService.PlaywrightTests.PageObjectModels;

namespace AuctionService.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class HomePageTests : ContextTestWithConfiguration
{
    [Test]
    public async Task Homepage_Should_Return3Cards_When_FordIsUsedAsASearchTerm()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var homepage = new HomePage(page, Configuration["auctionSvcUrl"]!);
        await homepage.GotoAsync();
        await homepage.SearchAsync("ford");

        await Expect(homepage.AuctionCard).ToHaveCountAsync(3);
    }

    [Test]
    public async Task Homepage_Should_Return1Card_When_FilteredByCompleted()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var homepage = new HomePage(page, Configuration["auctionSvcUrl"]!);
        await homepage.GotoAsync();
        await homepage.FilterByCompleted.ClickAsync();

        await Expect(homepage.AuctionCard).ToHaveCountAsync(1);
        await Expect(homepage.AuctionCardMakeModelText).ToHaveTextAsync("Mercedes SLK");
    }

    [Test]
    public async Task Homepage_Should_Return9Cards_When_FilteredByLiveAuctions()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var homepage = new HomePage(page, Configuration["auctionSvcUrl"]!);
        await homepage.GotoAsync();
        await homepage.FilterByLiveAuctions.ClickAsync();

        await Expect(homepage.AuctionCard).ToHaveCountAsync(9);
    }

    [Test]
    public async Task Homepage_Should_Return0Cards_When_FilteredByEndingSoon()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var homepage = new HomePage(page, Configuration["auctionSvcUrl"]!);
        await homepage.GotoAsync();
        await homepage.FilterByEndingSoon.ClickAsync();

        await Expect(homepage.AuctionCard).ToHaveCountAsync(0);
    }
}
