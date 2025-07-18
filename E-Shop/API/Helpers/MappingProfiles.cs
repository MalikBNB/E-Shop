﻿using API.DTOs;
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

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

            CreateMap<ShoppingCartDto, ShoppingCart>();

            CreateMap<CartItemDto, CartItem>();

            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod,
                           o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice,
                           o => o.MapFrom(s => s.DeliveryMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId,
                           o => o.MapFrom(s => s.itemOrdered.ProductItemId))
                .ForMember(d => d.ProductName,
                           o => o.MapFrom(s => s.itemOrdered.ProductName))
                .ForMember(d => d.PictureUrl,
                           o => o.MapFrom(s => s.itemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl,
                           o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}
