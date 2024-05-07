using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record ProductRequest : RequestObject
    {
        public ProductRequest(Guid id, string name, string brand, decimal price)
            : base(id)
        {
            Name = name;
            Brand = brand;
            Price = price;
        }

        public string Name { get; private set; }

        public string Brand { get; private set; }

        public decimal Price { get; private set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
    }
}
