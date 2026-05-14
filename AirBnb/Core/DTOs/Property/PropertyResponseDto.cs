namespace AirBnb.Core.DTOs.Property
{
    public class PropertyResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string? ImageUrl { get; set; }
        public string HostName { get; set; } = string.Empty;
        public string? HostId { get; set; }
    }
}
