using Xunit;
using Moq;
using FluentAssertions;
using Service;
using Domain.Models;
using Repository;
using Integration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ItemServiceTests
{
    private readonly Mock<IItemRepository> _repo;
    private readonly Mock<ICurrencyService> _currency;
    private readonly Mock<ILogger<ItemService>> _logger;
    private readonly ItemService _service;

    public ItemServiceTests()
    {
        _repo = new Mock<IItemRepository>();
        _currency = new Mock<ICurrencyService>();
        _logger = new Mock<ILogger<ItemService>>();

        _service = new ItemService(
            _repo.Object,
            _currency.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task GetAllItems_ShouldApplyCurrencyConversion_WhenCurrencyProvided()
    {
        var items = new List<Item>
        {
            CreateItem(1, 10m),
            CreateItem(2, 20m)
        };

        _repo.Setup(r => r.GetAllItems()).ReturnsAsync(items);
        _currency.Setup(c => c.GetExchangeToAsync("USD")).ReturnsAsync(2m);

        var result = await _service.GetAllItems("USD");

        result.Should().HaveCount(2);
        result.Should().Contain(i => i.BaseDailyPrice == 20m);
        result.Should().Contain(i => i.BaseDailyPrice == 40m);
    }

    [Fact]
    public async Task GetAllItems_ShouldReturnOriginalItems_WhenCurrencyNull()
    {
        var items = new List<Item> { CreateItem(1, 10m) };

        _repo.Setup(r => r.GetAllItems()).ReturnsAsync(items);

        var result = await _service.GetAllItems(null);

        result.Should().ContainSingle()
              .Which.BaseDailyPrice.Should().Be(10m);

        _currency.Verify(x => x.GetExchangeToAsync(It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task GetItemById_ShouldApplyConversion_WhenCurrencyProvided()
    {
        var item = CreateItem(1, 10m);

        _repo.Setup(r => r.GetItemById(1)).ReturnsAsync(item);
        _currency.Setup(c => c.GetExchangeToAsync("EUR")).ReturnsAsync(3m);

        var result = await _service.GetItemById(1, "EUR");

        result.BaseDailyPrice.Should().Be(30m);
    }

    [Fact]
    public async Task GetItemById_ShouldNotConvert_WhenCurrencyNull()
    {
        var item = CreateItem(1, 10m);

        _repo.Setup(r => r.GetItemById(1)).ReturnsAsync(item);

        var result = await _service.GetItemById(1, null);

        result.BaseDailyPrice.Should().Be(10m);

        _currency.Verify(x => x.GetExchangeToAsync(It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task CreateItem_ShouldConvertPrice_WhenCurrencyProvided()
    {
        var item = CreateItem(0, 10m);
        var created = CreateItem(1, 10m);

        _repo.Setup(r => r.CreateItem(item)).ReturnsAsync(created);
        _currency.Setup(c => c.GetExchangeFromAsync("USD")).ReturnsAsync(2m);

        var result = await _service.CreateItem(item, "USD");

        result.BaseDailyPrice.Should().Be(20m);
    }

    [Fact]
    public async Task CreateItem_ShouldNotConvert_WhenCurrencyNull()
    {
        var item = CreateItem(0, 10m);
        var created = CreateItem(1, 10m);

        _repo.Setup(r => r.CreateItem(item)).ReturnsAsync(created);

        var result = await _service.CreateItem(item, null);

        result.BaseDailyPrice.Should().Be(10m);

        _currency.Verify(x => x.GetExchangeFromAsync(It.IsAny<string>()), Times.Never);
    }


    [Fact]
    public async Task UpdateItem_ShouldCallRepositoryUpdate()
    {
        var original = CreateItem(1, 10m);
        var updated = CreateItem(1, 20m);

        var updateInput = CreateItem(0, 20m);

        _repo.Setup(r => r.GetItemById(1)).ReturnsAsync(original);
        _repo.Setup(r => r.UpdateItem(1, It.IsAny<Item>())).ReturnsAsync(updated);

        var result = await _service.UpdateItem(1, updateInput);

        result.BaseDailyPrice.Should().Be(20m);

        _repo.Verify(r => r.UpdateItem(1, It.IsAny<Item>()), Times.Once);
    }


    [Fact]
    public async Task DeleteItem_ShouldCallRepositoryDelete()
    {
        _repo.Setup(r => r.DeleteItem(5)).Returns(Task.CompletedTask);

        await _service.DeleteItem(5);

        _repo.Verify(r => r.DeleteItem(5), Times.Once);
    }

    private Item CreateItem(int id, decimal price)
    {
        var item = new Item();

        item.ChangeBaseDailyPrice(price);
        item.ChangeName("Test");
        item.ChangeCategory("TestCat");
        item.ChangeSecurityDeposit(0);
        item.ChangeTotalStock(1);
        item.SetActiveStatus(true);

        typeof(Item)
            .GetProperty("Id")!
            .SetValue(item, id);

        return item;
    }
}