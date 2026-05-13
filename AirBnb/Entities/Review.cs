namespace AirBnb.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public string? GuestId { get; set; }
        public User Guest { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
