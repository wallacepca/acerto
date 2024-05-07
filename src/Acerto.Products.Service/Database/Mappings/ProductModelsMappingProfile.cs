using Acerto.Products.Service.Database.Models;
using Acerto.Shared.Contracts;
using AutoMapper;

namespace Acerto.Products.Service.Database.Mappings
{
    public sealed class ProductModelsMappingProfile : Profile
    {
        public ProductModelsMappingProfile()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<ProductRequest, Product>();
        }
    }
}
