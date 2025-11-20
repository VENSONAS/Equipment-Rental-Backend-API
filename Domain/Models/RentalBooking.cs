using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RentalBooking
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int ItemId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public Status BookingStatus { get; private set; }
        public int Quantity { get; private set; }
        
        private decimal _calculatedPrice;
        public decimal CalculatedPrice
        {
            get => Math.Round(_calculatedPrice, 2, MidpointRounding.AwayFromZero);
            private set => _calculatedPrice = value;
        }
        public DateTime CreatedAt { get; private set; }

        public RentalBooking() {}

        public void ChangeStartDate(DateTime newStartDate)
        {
            StartDate = newStartDate;
        }

        public void ChangeEndDate(DateTime newEndDate)
        {
            EndDate = newEndDate;
        }

        public void ChangeQuantity(int newQuantity)
        {
            Quantity = newQuantity;
        }

        public void ConvertPrice(decimal conversionRate)
        {
            CalculatedPrice *= conversionRate;
        }

        public void ConfirmBooking()
        {
            if(BookingStatus != Status.Pending)
                throw new InvalidOperationException("Only pending bookings can be confirmed.");
            BookingStatus = Status.Confirmed;
        }
        public void CompleteBooking()
        {
            if(BookingStatus != Status.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");
            BookingStatus = Status.Completed;
        }
        public void CancelBooking()
        {
            if(BookingStatus == Status.Completed)
                throw new InvalidOperationException("Completed bookings cannot be cancelled.");
            BookingStatus = Status.Cancelled;
        }

        public void SetCreatedTime()
        {
            CreatedAt = DateTime.UtcNow;
        }
        
        public bool ValidateDates()
        {
            return EndDate > StartDate;
        }

        public void CalculatePrice(decimal baseDailyPrice)
        {
            int rentalDays = (EndDate - StartDate).Days;
            CalculatedPrice = rentalDays * baseDailyPrice * Quantity;
        }
    }
    public enum Status
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
