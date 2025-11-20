using Domain.Models;

namespace Repository.Entities
{
    public class RentalBookingEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status BookingStatus { get; set; }
        public int Quantity { get; set; }
        public decimal CalculatedPrice { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
