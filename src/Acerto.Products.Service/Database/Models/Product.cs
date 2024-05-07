using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Products.Service.Database.Models
{
    public class Product : Entity, IHasCreationDate, IHasUpdateDate
    {
        public Product(string name, string brand, decimal price)
        {
            Name = name;
            Brand = brand;
            Price = price;
        }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
