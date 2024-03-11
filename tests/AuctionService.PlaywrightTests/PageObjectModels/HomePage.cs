using Microsoft.Playwright;

namespace AuctionService.PlaywrightTests.PageObjectModels;

public class HomePage
{
    private readonly IPage _page;
    private readonly string _url;
    private readonly ILocator _searchTermInput;
    public ILocator AuctionCard { get; private set; }
    public ILocator AuctionCardMakeModelText { get; private set; }

    #region filter by locators 
    public ILocator FilterByLiveAuctions { get; private set; }
    public ILocator FilterByEndingSoon { get; private set; }
    public ILocator FilterByCompleted { get; private set; }
    #endregion
    #region order by locators 
    public ILocator OrderByAlphabetical { get; private set; }
    public ILocator OrderByEndDate { get; private set; }
    public ILocator OrderByRecentlyAdded { get; private set; }
    #endregion

    public HomePage(IPage page, string url)
    {
        _page = page;
        _url = url;
        _searchTermInput = _page.GetByTestId("search-input");
        AuctionCard = _page.GetByTestId("auction-card");
        AuctionCardMakeModelText = _page.GetByTestId("make-model-text");
        #region filter by locators 
        FilterByLiveAuctions = _page.GetByTestId("filter-by-live-auctions");
        FilterByEndingSoon = _page.GetByTestId("filter-by-ending-soon");
        FilterByCompleted = _page.GetByTestId("filter-by-completed");
        #endregion

        #region order by locators 
        OrderByAlphabetical = _page.GetByTestId("order-by-alphabetical");
        OrderByEndDate = _page.GetByTestId("order-by-end-date");
        OrderByRecentlyAdded = _page.GetByTestId("order-by-recently-added");
        #endregion
    }

    public async Task GotoAsync()
    {
        await _page.GotoAsync(_url);
    }

    public async Task SearchAsync(string text)
    {
        await _searchTermInput.FillAsync(text);
        await _searchTermInput.PressAsync("Enter");
    }
}
