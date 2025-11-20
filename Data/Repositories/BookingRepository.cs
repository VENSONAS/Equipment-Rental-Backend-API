using Domain.Models;
using Repository.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RentalBooking> CreateBooking(RentalBooking booking)
        {
            var entity = booking.Adapt<Repository.Entities.RentalBookingEntity>();
            await _context.RentalBookings.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Adapt<RentalBooking>();
        }

        public async Task DeleteBooking(int id)
        {
            var booking = await _context.RentalBookings.FindAsync(id);
            if (booking != null)
            {
                _context.RentalBookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RentalBooking>> GetAllBookings()
        {
            var bookings = await _context.RentalBookings.ToListAsync();
            return bookings.Adapt<List<RentalBooking>>();
        }

        public async Task<RentalBooking> GetBookingById(int id)
        {
            var booking = await _context.RentalBookings.FindAsync(id);
            return booking.Adapt<RentalBooking>();
        }

        public async Task<IEnumerable<RentalBooking>> GetBookingsByItemAndDateRange(int ItemId, DateTime Start, DateTime End)
        {
            var bookings = await _context.RentalBookings
                .Where(b => b.ItemId == ItemId &&
                            b.StartDate < End &&
                            b.EndDate > Start &&
                            b.BookingStatus != Status.Cancelled)
                .ToListAsync();

            return bookings.Adapt<List<RentalBooking>>();
        }

        public async Task<RentalBooking> UpdateBooking(int id, RentalBooking booking)
        {
            var existingBooking = await _context.RentalBookings.FindAsync(id);
            booking.Adapt(existingBooking);
            await _context.SaveChangesAsync();
            return existingBooking.Adapt<RentalBooking>();
        }
    }
}
