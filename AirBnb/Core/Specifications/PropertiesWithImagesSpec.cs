using AirBnb.Core.Entities;

namespace AirBnb.Core.Specifications
{
    public class PropertiesWithImagesSpec : BaseSpecification<Property>
    {
        public PropertiesWithImagesSpec()
        {
            AddInclude(p => p.Host);
        }

        public PropertiesWithImagesSpec(int id) : this()
        {
            AddCriteria(p => p.Id == id);
        }
    }
}
