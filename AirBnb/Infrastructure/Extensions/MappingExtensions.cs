using AirBnb.Core.DTOs.Booking;
using AirBnb.Core.DTOs.Payment;
using AirBnb.Core.DTOs.Property;
using AirBnb.Core.DTOs.Review;
using AirBnb.Core.Entities;

namespace AirBnb.Infrastructure.Extensions
{
    public static class MappingExtensions
    {
        public static PropertyResponseDto ToResponseDto(this Property p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            PricePerNight = p.PricePerNight,
            ImageUrl = p.ImageUrl,
            HostName = p.Host?.FullName ?? string.Empty,
            HostId = p.HostId
        };

        public static BookingResponseDto ToResponseDto(this Booking b) => new()
        {
            Id = b.Id,
            PropertyId = b.PropertyId,
            PropertyTitle = b.Property?.Title ?? string.Empty,
            GuestId = b.GuestId ?? string.Empty,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            TotalAmount = b.TotalAmount,
            IsPaid = b.IsPaid
        };

        public static PaymentResponseDto ToResponseDto(this Payment p) => new()
        {
            Id = p.Id,
            BookingId = p.BookingId,
            Amount = p.Amount,
            PaymentStatus = p.PaymentStatus,
            PaymentDate = p.PaymentDate
        };

        public static ReviewResponseDto ToResponseDto(this Review r) => new()
        {
            Id = r.Id,
            PropertyId = r.PropertyId,
            GuestId = r.GuestId ?? string.Empty,
            Rating = r.Rating,
            Comment = r.Comment
        };
    }
}
