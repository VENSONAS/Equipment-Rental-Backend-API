using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Repository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<RentalBooking>> GetAllBookings();
        Task<RentalBooking> GetBookingById(int id);
        Task<RentalBooking> CreateBooking(RentalBooking booking);
        Task<RentalBooking> UpdateBooking(int id, RentalBooking booking);
        Task DeleteBooking(int id);
        Task<IEnumerable<RentalBooking>> GetBookingsByItemAndDateRange(int ItemId, DateTime Start, DateTime End);
    }
}
