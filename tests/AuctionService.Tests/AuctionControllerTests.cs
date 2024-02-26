using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AuctionService.Tests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace AuctionService.Tests;

public class AuctionControllerTests
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly AuctionsController _controller;

    public AuctionControllerTests()
    {
        _auctionRepository = Substitute.For<IAuctionRepository>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _mapper = new Mapper(
            new MapperConfiguration(mc =>
            {
                mc.AddMaps(typeof(MappingProfiles).Assembly);
            }).CreateMapper().ConfigurationProvider);
        _fixture = new Fixture();
        _controller = new AuctionsController(_mapper, _publishEndpoint, _auctionRepository)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = Helpers.GetClaimsPrincipal()
                }
            }
        };
    }

    [Fact]
    public async Task GetAuction_WithNoParams_Returns10Auctions()
    {
        // arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepository.GetAuctionsAsync(Arg.Any<string>()).Returns(auctions);

        // act
        var result = await _controller.GetAllAuctions(null);

        // assert
        Assert.NotNull(result?.Value);
        Assert.Equal(10, result!.Value!.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        // arrange
        var auction = _fixture.Create<AuctionDto>();
        _auctionRepository.GetAuctionByIdAsync(Arg.Any<Guid>()).Returns(auction);

        // act
        var result = await _controller.GetAuctionById(auction.Id);

        // assert
        Assert.NotNull(result?.Value);
        Assert.Equal(auction.Make, result.Value.Make);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
    {
        // arrange
        var id = Guid.NewGuid();
        _auctionRepository.GetAuctionByIdAsync(id).ReturnsNull();

        // act
        var result = await _controller.GetAuctionById(id);

        // assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
    {
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepository.SaveChangesAsync().Returns(true);

        var result = await _controller.CreateAuction(auction);
        var createdAtResult = result.Result as CreatedAtActionResult;

        Assert.NotNull(createdAtResult);
        Assert.Equal("GetAuctionById", createdAtResult.ActionName);
        Assert.IsType<AuctionDto>(createdAtResult.Value);
    }

    [Fact]
    public async Task CreateAuction_FailedSave_Returns400BadRequest()
    {
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepository.SaveChangesAsync().Returns(false);

        var result = await _controller.CreateAuction(auction);
        var badRequestObjectResult = result.Result as BadRequestObjectResult;

        Assert.NotNull(badRequestObjectResult);
        Assert.Equal("Could not save changes to the DB", badRequestObjectResult.Value);
    }

    [Fact]
    public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsOkResponse()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).Returns(auction);
        _auctionRepository.SaveChangesAsync().Returns(true);

        var result = await _controller.UpdateAuction(Guid.NewGuid(), new UpdateAuctionDto());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_Returns403Forbid()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).Returns(auction);
        _auctionRepository.SaveChangesAsync().Returns(true);

        var result = await _controller.UpdateAuction(Guid.NewGuid(), new UpdateAuctionDto());

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
    {
        _auctionRepository.SaveChangesAsync().Returns(false);

        var result = await _controller.UpdateAuction(Guid.NewGuid(), new UpdateAuctionDto());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidUser_ReturnsOkResponse()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).Returns(auction);
        _auctionRepository.SaveChangesAsync().Returns(true);

        var result = await _controller.DeleteAuction(Guid.NewGuid());

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidGuid_Returns404Response()
    {
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).ReturnsNull();

        var result = await _controller.DeleteAuction(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidUser_Returns403Response()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).Returns(auction);
        _auctionRepository.SaveChangesAsync().Returns(true);

        var result = await _controller.DeleteAuction(Guid.NewGuid());

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task RemoveAuction_FailedSave_Returns400BadRequest()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        _auctionRepository.GetAuctionEntityById(Arg.Any<Guid>()).Returns(auction);
        _auctionRepository.SaveChangesAsync().Returns(false);

        var result = await _controller.DeleteAuction(Guid.NewGuid());
        var badRequestObjectResult = result as BadRequestObjectResult;

        Assert.NotNull(badRequestObjectResult);
        Assert.Equal("Delete unsuccessful", badRequestObjectResult.Value);
    }
}
