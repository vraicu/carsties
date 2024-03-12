using Microsoft.Playwright;

namespace AuctionService.PlaywrightTests.PageObjectModels;

public class AuctionDetailsPage
{
    private readonly IPage _page;
    private readonly string _url;

    public ILocator AuctionCard { get; private set; }
    public ILocator EditButton { get; private set; }
    public ILocator DeleteButton { get; private set; }
    public ILocator BidFormMessagePlaceholder { get; private set; }
    public ILocator AuctionDetailsSeller { get; private set; }
    public ILocator AuctionDetailsMake { get; private set; }
    public ILocator AuctionDetailsModel { get; private set; }
    public ILocator AuctionDetailsYear { get; private set; }
    public ILocator AuctionDetailsMileage { get; private set; }
    public ILocator AuctionDetailsHasReservePrice { get; private set; }

    public AuctionDetailsPage(IPage page, string url)
    {
        _page = page;
        _url = url;

        AuctionCard = _page.GetByTestId("auction-card");
        EditButton = _page.GetByTestId("auction-details-edit-button");
        DeleteButton = _page.GetByTestId("auction-details-delete-button");
        BidFormMessagePlaceholder = _page.GetByTestId("bid-message");
        AuctionDetailsSeller = _page.GetByTestId("auction-details-seller");
        AuctionDetailsModel = _page.GetByTestId("auction-details-model"); AuctionDetailsMake = _page.GetByTestId("auction-details-make"); AuctionDetailsYear = _page.GetByTestId("auction-details-year"); AuctionDetailsMileage = _page.GetByTestId("auction-details-mileage"); AuctionDetailsHasReservePrice = _page.GetByTestId("auction-details-has-reserve-price");
    }

    public async Task GotoAsync()
    {
        await _page.GotoAsync(_url);
    }
}
