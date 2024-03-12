using AuctionService.PlaywrightTests.Fixtures;
using AuctionService.PlaywrightTests.PageObjectModels;

namespace AuctionService.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AuctionDetailsPageTests : ContextTestWithConfiguration
{
    [Test]
    public async Task AuctionDetailsPage_Should_HideEditAndDeleteButton_When_UserNotLoggedIn()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var auctionDetailsPage = new AuctionDetailsPage(page, Configuration["auctionSvcUrl"]!);
        await auctionDetailsPage.GotoAsync();
        await auctionDetailsPage.AuctionCard.Nth(0).ClickAsync();

        await Expect(auctionDetailsPage.EditButton).ToBeHiddenAsync();
        await Expect(auctionDetailsPage.DeleteButton).ToBeHiddenAsync();
    }

    [Test]
    public async Task AuctionDetailsPage_Should_HideBidForm_When_UserNotLoggedIn()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var auctionDetailsPage = new AuctionDetailsPage(page, Configuration["auctionSvcUrl"]!);
        await auctionDetailsPage.GotoAsync();
        await auctionDetailsPage.AuctionCard.Nth(0).ClickAsync();

        await Expect(auctionDetailsPage.BidFormMessagePlaceholder).ToHaveTextAsync("Please login to make a bid");
    }

    [Test]
    public async Task AuctionDetailsPage_Should_DisplayAuctionDetails()
    {
        var page = await base.Context.NewPageAsync().ConfigureAwait(continueOnCapturedContext: false);
        var auctionDetailsPage = new AuctionDetailsPage(page, Configuration["auctionSvcUrl"]!);
        await auctionDetailsPage.GotoAsync();
        await auctionDetailsPage.AuctionCard.Nth(0).ClickAsync();

        await Expect(auctionDetailsPage.AuctionDetailsSeller).ToHaveTextAsync("bob");
        await Expect(auctionDetailsPage.AuctionDetailsYear).ToHaveTextAsync("2021");
        await Expect(auctionDetailsPage.AuctionDetailsModel).ToHaveTextAsync("R8");
        await Expect(auctionDetailsPage.AuctionDetailsMake).ToHaveTextAsync("Audi");
        await Expect(auctionDetailsPage.AuctionDetailsMileage).ToHaveTextAsync("10050");
        await Expect(auctionDetailsPage.AuctionDetailsHasReservePrice).ToHaveTextAsync("No");
    }
}
