using Xunit;
using Moq;
using FluentAssertions;
using Service;
using Domain.Models;
using Repository;
using Integration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.ServiceTests
{
    public class RentalBookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookings;
        private readonly Mock<IItemRepository> _items;
        private readonly Mock<ICurrencyService> _currency;
        private readonly Mock<ILogger<RentalBookingService>> _logger;
        private readonly RentalBookingService _service;

        public RentalBookingServiceTests()
        {
            _bookings = new Mock<IBookingRepository>();
            _items = new Mock<IItemRepository>();
            _currency = new Mock<ICurrencyService>();
            _logger = new Mock<ILogger<RentalBookingService>>();

            _service = new RentalBookingService(
                _bookings.Object,
                _items.Object,
                _currency.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task CancelRentalBooking_ShouldCancel_AndUpdateRepository()
        {
            var booking = CreateBooking(Status.Pending);

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(booking);

            await _service.CancelRentalBooking(1);

            booking.BookingStatus.Should().Be(Status.Cancelled);
            _bookings.Verify(r => r.UpdateBooking(1, booking), Times.Once);
        }

        [Fact]
        public async Task CancelRentalBooking_ShouldThrow_WhenNotFound()
        {
            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync((RentalBooking)null);

            Func<Task> act = async () => await _service.CancelRentalBooking(1);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Booking not found.");
        }

        [Fact]
        public async Task ConfirmRentalBooking_ShouldConfirm()
        {
            var booking = CreateBooking(Status.Pending);

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(booking);

            await _service.ConfirmRentalBooking(1);

            booking.BookingStatus.Should().Be(Status.Confirmed);
            _bookings.Verify(r => r.UpdateBooking(1, booking), Times.Once);
        }


        [Fact]
        public async Task CompleteRentalBooking_ShouldComplete()
        {
            var booking = CreateBooking(Status.Confirmed);

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(booking);

            await _service.CompleteRentalBooking(1);

            booking.BookingStatus.Should().Be(Status.Completed);
            _bookings.Verify(r => r.UpdateBooking(1, booking), Times.Once);
        }


        [Fact]
        public async Task CreateRentalBooking_ShouldThrow_WhenStockInsufficient()
        {
            var booking = CreateBooking(Status.Pending, quantity: 3);

            _bookings.Setup(r =>
                r.GetBookingsByItemAndDateRange(
                    booking.ItemId,
                    booking.StartDate,
                    booking.EndDate))
            .ReturnsAsync(new List<RentalBooking>
            {
                CreateBooking(Status.Pending, quantity: 3)
            });

            _items.Setup(r => r.GetItemById(It.IsAny<int>()))
                  .ReturnsAsync((int id) => CreateItem(id, 10m, totalStock: 5));

            Func<Task> act = async () => await _service.CreateRentalBooking(booking, null);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Not enough items available for the selected date range.");
        }

        [Fact]
        public async Task CreateRentalBooking_ShouldCalculatePrice_AndConvert_WhenCurrencyProvided()
        {
            var booking = CreateBooking(Status.Pending, quantity: 2);

            _bookings.Setup(r =>
                r.GetBookingsByItemAndDateRange(booking.ItemId, booking.StartDate, booking.EndDate)
            ).ReturnsAsync(new List<RentalBooking>());

            _items.Setup(r => r.GetItemById(It.IsAny<int>()))
                  .ReturnsAsync((int id) => CreateItem(id, 10m, 100));

            _currency.Setup(r => r.GetExchangeFromAsync("USD")).ReturnsAsync(2m);

            _bookings.Setup(r => r.CreateBooking(booking)).ReturnsAsync(booking);

            var result = await _service.CreateRentalBooking(booking, "USD");

            result.CalculatedPrice.Should().Be(40m);
        }


        [Fact]
        public async Task GetRentalBookingById_ShouldConvert_WhenCurrencyProvided()
        {
            var booking = CreateBooking(Status.Pending);
            booking.CalculatePrice(10m); // 10

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(booking);
            _currency.Setup(c => c.GetExchangeToAsync("EUR")).ReturnsAsync(3m);

            var result = await _service.GetRentalBookingById(1, "EUR");

            result.CalculatedPrice.Should().Be(30m);
        }

        [Fact]
        public async Task UpdateRentalBooking_ShouldThrow_WhenInvalidDates()
        {
            var updated = CreateBooking(Status.Pending);
            updated.ChangeEndDate(updated.StartDate.AddDays(-1));

            Func<Task> act = () => _service.UpdateRentalBooking(1, updated);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Invalid booking dates.");
        }


        [Fact]
        public async Task UpdateRentalBooking_ShouldThrow_WhenStockInsufficient()
        {
            var original = CreateBooking(Status.Pending, quantity: 2);
            var updated = CreateBooking(Status.Pending, quantity: 5);

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(original);

            _bookings.Setup(r =>
                r.GetBookingsByItemAndDateRange(
                    original.ItemId,
                    updated.StartDate,
                    updated.EndDate)
            ).ReturnsAsync(new List<RentalBooking>
            {
                CreateBooking(Status.Pending, quantity: 6)
            });

            _items.Setup(r => r.GetItemById(It.IsAny<int>()))
                  .ReturnsAsync((int id) => CreateItem(id, 10m, totalStock: 5));

            Func<Task> act = () => _service.UpdateRentalBooking(1, updated);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Not enough items available for the selected date range.");
        }

        [Fact]
        public async Task UpdateRentalBooking_ShouldUpdate_AndRecalculatePrice()
        {
            var original = CreateBooking(Status.Pending, quantity: 1);
            var updated = CreateBooking(Status.Pending, quantity: 2);

            _bookings.Setup(r => r.GetBookingById(1)).ReturnsAsync(original);

            _bookings.Setup(r =>
                r.GetBookingsByItemAndDateRange(
                    original.ItemId,
                    updated.StartDate,
                    updated.EndDate)
            ).ReturnsAsync(new List<RentalBooking>());

            _items.Setup(r => r.GetItemById(It.IsAny<int>()))
                  .ReturnsAsync((int id) => CreateItem(id, 10m, 100));

            _bookings.Setup(r => r.UpdateBooking(1, original)).ReturnsAsync(original);

            var result = await _service.UpdateRentalBooking(1, updated);

            result.CalculatedPrice.Should().Be(20m);
        }


        private RentalBooking CreateBooking(Status status, int quantity = 1)
        {
            var b = new RentalBooking();

            b.ChangeStartDate(DateTime.Today);
            b.ChangeEndDate(DateTime.Today.AddDays(1));
            b.ChangeQuantity(quantity);

            typeof(RentalBooking).GetProperty("BookingStatus")!.SetValue(b, status);
            typeof(RentalBooking).GetProperty("ItemId")!.SetValue(b, 1);
            typeof(RentalBooking).GetProperty("UserId")!.SetValue(b, 1);
            typeof(RentalBooking).GetProperty("Id")!.SetValue(b, 1);

            return b;
        }

        private Item CreateItem(int id, decimal price = 10m, int totalStock = 10)
        {
            var item = new Item();
            item.ChangeBaseDailyPrice(price);
            item.ChangeTotalStock(totalStock);
            item.ChangeName("Test");
            item.ChangeCategory("Cat");
            item.ChangeSecurityDeposit(0);
            item.SetActiveStatus(true);

            typeof(Item).GetProperty("Id")!.SetValue(item, id);

            return item;
        }
    }
}
