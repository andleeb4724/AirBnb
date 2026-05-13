namespace AirBnb.Dtos
{
    public class CreatePropertyDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class UpdatePropertyDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public IFormFile ImageFile { get; set; }
    }

}
