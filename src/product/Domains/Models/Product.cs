using ProductApi.Domains.Dtos.Product;

namespace ProductApi.Domains.Models
{
    public class Product
    {
        public Product()
        {
            
        }

        public Product(ProductDto dto)
        {
            this.Id = dto.Id;
            this.Name = dto.Name;
            this.Value = dto.Value;
            this.CreateDate = dto.CreateDate;
        }

        public Product(ProductCreateDto dto)
        {
            this.Name = dto.Name;
            this.Value = dto.Value;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime CreateDate { get; set; }

        internal ProductDto ToDto()
        {
            return new ProductDto(Id, Name, Value, CreateDate);
        }
    }
}
