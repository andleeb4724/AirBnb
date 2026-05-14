namespace AirBnb.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public string? GuestId { get; set; }
        public User Guest { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
