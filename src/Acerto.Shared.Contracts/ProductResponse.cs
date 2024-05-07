using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record ProductResponse : ResponseObject
    {
        public ProductResponse(Guid id, string name, string brand, decimal price)
            : base(id)
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
        public static ProductResponse Empty => new(Guid.Empty, string.Empty, string.Empty, 0);
    }
}
