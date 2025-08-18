using API.DTOs;
using API.DTOs.Identity;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.ProductBrand,
                           o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType,
                           o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl,
                           o => o.MapFrom<ProductUrlResolver>());

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<ShoppingCartDto, ShoppingCart>();

            CreateMap<CartItemDto, CartItem>();

            CreateMap<AddressDto, ShippingAddress>();

            CreateMap<Order, OrderDto>()
                .ForMember(d => d.DeliveryMethod,
                           o => o.MapFrom(s => s.DeliveryMethod.Description))
                .ForMember(d => d.ShippingPrice,
                           o => o.MapFrom(s => s.DeliveryMethod.Price))
                .ForMember(d => d.Status,
                           o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Total,
                           o => o.MapFrom(s => s.GetTotal()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId,
                           o => o.MapFrom(s => s.ItemOrdered.ProductId))
                .ForMember(d => d.ProductName,
                           o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl,
                           o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl,
                           o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}
