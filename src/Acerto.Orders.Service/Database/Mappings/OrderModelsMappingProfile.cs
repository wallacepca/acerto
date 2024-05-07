using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Contracts;
using AutoMapper;

namespace Acerto.Orders.Service.Database.Mappings
{
    public sealed class OrderModelsMappingProfile : Profile
    {
        public OrderModelsMappingProfile()
        {
            CreateMap<Order, OrderResponse>();
            CreateMap<OrderRequest, Order>();

            CreateMap<OrderItem, OrderItemResponse>();
            CreateMap<OrderItemRequest, OrderItem>();

            CreateMap<OrderStatusChange, OrderStatusChangeResponse>();
        }
    }
}
