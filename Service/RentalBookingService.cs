using Domain.Models;
using Integration;
using Microsoft.Extensions.Logging;
using Repository;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class RentalBookingService : IRentalBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<RentalBookingService> _logger;

        public RentalBookingService(IBookingRepository rentalBookingRepository, IItemRepository itemRepository, ICurrencyService currencyService, ILogger<RentalBookingService> logger)
        {
            _bookingRepository = rentalBookingRepository;
            _itemRepository = itemRepository;
            _currencyService = currencyService;
            _logger = logger;
        }

        public async Task CancelRentalBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }
            booking.CancelBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task CompleteRentalBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }
            booking.CompleteBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task ConfirmRentalBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }
            booking.ConfirmBooking();
            await _bookingRepository.UpdateBooking(booking.Id, booking);
        }

        public async Task<RentalBooking> CreateRentalBooking(RentalBooking rentalBooking, string? currency)
        {
            var overlappingBookings = await _bookingRepository.GetBookingsByItemAndDateRange(
                rentalBooking.ItemId,
                rentalBooking.StartDate,
                rentalBooking.EndDate);

            var itemToRent = await _itemRepository.GetItemById(rentalBooking.ItemId);

            if (overlappingBookings.Sum(b => b.Quantity) + rentalBooking.Quantity > itemToRent.TotalStock)
            {
                throw new InvalidOperationException("Not enough items available for the selected date range.");
            }
            if (!rentalBooking.ValidateDates())
            {
                throw new InvalidOperationException("Invalid booking dates.");
            }

            rentalBooking.CalculatePrice(itemToRent.BaseDailyPrice);

            if (currency != null)
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
            await _bookingRepository.DeleteBooking(id);
        }

        public async Task<IEnumerable<RentalBooking>> GetAllRentalBookings(string? currency)
        {
            var bookings = await _bookingRepository.GetAllBookings();
            if (currency == null) return bookings;

            var conversionRate = await _currencyService.GetExchangeToAsync(currency);
            foreach (var booking in bookings)
            {
                booking.ConvertPrice(conversionRate);
            }
            return bookings;
        }

        public async Task<RentalBooking> GetRentalBookingById(int id, string? currency)
        {
            var booking = await _bookingRepository.GetBookingById(id);
            if (currency != null)
            {
                var conversionRate = await _currencyService.GetExchangeToAsync(currency);
                booking.ConvertPrice(conversionRate);
            }
            return booking;
        }

        public async Task<RentalBooking> UpdateRentalBooking(int id, RentalBooking rentalBooking)
        {

            if (!rentalBooking.ValidateDates())
            {
                throw new InvalidOperationException("Invalid booking dates.");
            }

            var bookingToUpdate = await _bookingRepository.GetBookingById(id);

            var overlappingBookings = await _bookingRepository.GetBookingsByItemAndDateRange(
                bookingToUpdate.ItemId,
                rentalBooking.StartDate,
                rentalBooking.EndDate);


            var itemToRent = await _itemRepository.GetItemById(bookingToUpdate.ItemId);

            if (overlappingBookings.Sum(b => b.Quantity) - bookingToUpdate.Quantity + rentalBooking.Quantity > itemToRent.TotalStock)
            {
                throw new InvalidOperationException("Not enough items available for the selected date range.");
            }       

            bookingToUpdate.ChangeStartDate(rentalBooking.StartDate);
            bookingToUpdate.ChangeEndDate(rentalBooking.EndDate);
            bookingToUpdate.ChangeQuantity(rentalBooking.Quantity);

            bookingToUpdate.CalculatePrice(itemToRent.BaseDailyPrice);

            return await _bookingRepository.UpdateBooking(bookingToUpdate.Id ,bookingToUpdate);
        }
    }
}
