using API.DTOs;
using AutoMapper;
using Core.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductResponseDto, string>
    {
        private readonly IConfiguration _config;

        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Product source, ProductResponseDto destination, 
            string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.PictureUrl)) return null!;

            return _config["ApiUrl"] + source.PictureUrl;
        }
    }
}
