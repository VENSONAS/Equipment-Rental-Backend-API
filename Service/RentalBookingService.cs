using Domain.Models;
using Integration;
using Microsoft.Extensions.Logging;
using Repository;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class RentalBookingService : IRentalBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<RentalBookingService> _logger;

        public RentalBookingService(
            IBookingRepository rentalBookingRepository,
            IItemRepository itemRepository,
            ICurrencyService currencyService,
            ILogger<RentalBookingService> logger)
        {
            _bookingRepository = rentalBookingRepository ?? throw new ArgumentNullException(nameof(rentalBookingRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CancelRentalBooking(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException($"Booking with ID {id} not found.");
            }

            booking.CancelBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task CompleteRentalBooking(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException($"Booking with ID {id} not found.");
            }

            booking.CompleteBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task ConfirmRentalBooking(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException($"Booking with ID {id} not found.");
            }

            booking.ConfirmBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task<RentalBooking> CreateRentalBooking(RentalBooking rentalBooking, string? currency)
        {
            if (rentalBooking == null) throw new ArgumentNullException(nameof(rentalBooking));
            if (rentalBooking.ItemId <= 0) throw new InvalidOperationException("Invalid ItemId.");

            if (!rentalBooking.ValidateDates())
            {
                throw new InvalidOperationException("Invalid booking dates.");
            }

            var itemToRent = await _itemRepository.GetItemById(rentalBooking.ItemId);
            if (itemToRent == null)
            {
                throw new InvalidOperationException($"Item with ID {rentalBooking.ItemId} not found.");
            }

            var overlappingBookings = await _bookingRepository.GetBookingsByItemAndDateRange(
                rentalBooking.ItemId,
                rentalBooking.StartDate,
                rentalBooking.EndDate);

            if (overlappingBookings == null)
            {
                throw new InvalidOperationException("Failed to fetch overlapping bookings.");
            }

            if (overlappingBookings.Sum(b => b.Quantity) + rentalBooking.Quantity > itemToRent.TotalStock)
            {
                throw new InvalidOperationException("Not enough items available for the selected date range.");
            }

            rentalBooking.CalculatePrice(itemToRent.BaseDailyPrice);

            if (!string.IsNullOrWhiteSpace(currency))
            {
                var conversionRate = await _currencyService.GetExchangeFromAsync(currency);
                rentalBooking.ConvertPrice(conversionRate);
            }

            rentalBooking.SetCreatedTime();

            _logger.LogInformation($"Booking created ID: {rentalBooking.Id}");

            return await _bookingRepository.CreateBooking(rentalBooking);
        }

        public async Task DeleteRentalBooking(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            await _bookingRepository.DeleteBooking(id);
        }

        public async Task<IEnumerable<RentalBooking>> GetAllRentalBookings(string? currency)
        {
            var bookings = await _bookingRepository.GetAllBookings()
                ?? throw new InvalidOperationException("Failed to retrieve bookings.");

            if (string.IsNullOrWhiteSpace(currency))
            {
                return bookings;
            }

            var conversionRate = await _currencyService.GetExchangeToAsync(currency);

            foreach (var booking in bookings)
            {
                booking.ConvertPrice(conversionRate);
            }

            return bookings;
        }

        public async Task<RentalBooking> GetRentalBookingById(int id, string? currency)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var booking = await _bookingRepository.GetBookingById(id)
                ?? throw new InvalidOperationException($"Booking with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(currency))
            {
                var conversionRate = await _currencyService.GetExchangeToAsync(currency);
                booking.ConvertPrice(conversionRate);
            }

            return booking;
        }

        public async Task<RentalBooking> UpdateRentalBooking(int id, RentalBooking rentalBooking)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (rentalBooking == null) throw new ArgumentNullException(nameof(rentalBooking));
            if (!rentalBooking.ValidateDates())
            {
                throw new InvalidOperationException("Invalid booking dates.");
            }

            var bookingToUpdate = await _bookingRepository.GetBookingById(id)
                ?? throw new InvalidOperationException($"Booking with ID {id} not found.");

            var item = await _itemRepository.GetItemById(bookingToUpdate.ItemId)
                ?? throw new InvalidOperationException($"Item with ID {bookingToUpdate.ItemId} not found.");

            var overlappingBookings = await _bookingRepository.GetBookingsByItemAndDateRange(
                bookingToUpdate.ItemId,
                rentalBooking.StartDate,
                rentalBooking.EndDate);

            if (overlappingBookings == null)
            {
                throw new InvalidOperationException("Failed to fetch overlapping bookings.");
            }

            var adjustedQuantity =
                overlappingBookings.Sum(b => b.Quantity)
                - bookingToUpdate.Quantity
                + rentalBooking.Quantity;

            if (adjustedQuantity > item.TotalStock)
            {
                throw new InvalidOperationException("Not enough items available for the selected date range.");
            }

            bookingToUpdate.ChangeStartDate(rentalBooking.StartDate);
            bookingToUpdate.ChangeEndDate(rentalBooking.EndDate);
            bookingToUpdate.ChangeQuantity(rentalBooking.Quantity);
            bookingToUpdate.CalculatePrice(item.BaseDailyPrice);

            return await _bookingRepository.UpdateBooking(bookingToUpdate.Id, bookingToUpdate);
        }
    }
}
