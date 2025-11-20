using Domain.Models;

namespace Equipment_Rental_Backend_API.Dtos.RentalBooking
{
    public class RentalBookingDto
    {
        public int Id { get;  set; }
        public int UserId { get;  set; }
        public int ItemId { get;  set; }
        public DateTime StartDate { get;  set; }
        public DateTime EndDate { get;  set; }
        public required string BookingStatus { get; set; }
        public int Quantity { get;  set; }
        public decimal CalculatedPrice { get;  set; }
        public DateTime CreatedAt { get;  set; }
    }

    public class CreateRentalBookingDto
    {
        public int UserId { get; init; }
        public int ItemId { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int Quantity { get; init; }
    }

    public class UpdateRentalBookingDto
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int Quantity { get; init; }
    }
}
