using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IRentalBookingService
    {
        Task<IEnumerable<RentalBooking>> GetAllRentalBookings(string? currency);
        Task<RentalBooking> GetRentalBookingById(int id, string? currency);
        Task<RentalBooking> CreateRentalBooking(RentalBooking rentalBooking, string? currency);
        Task<RentalBooking> UpdateRentalBooking(int id, RentalBooking rentalBooking);
        Task DeleteRentalBooking(int id);
        Task ConfirmRentalBooking(int id);
        Task CancelRentalBooking(int id);
        Task CompleteRentalBooking(int id);
    }
}
