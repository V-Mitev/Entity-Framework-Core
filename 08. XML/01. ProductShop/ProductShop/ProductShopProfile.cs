using AutoMapper;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            // User

            CreateMap<ImportUserDto, User>();

            // Products 

            CreateMap<ImportProductDto, Product>()
                .ForMember(d => d.BuyerId.HasValue,
                opt => opt.MapFrom(s => s.BuyerId!.HasValue));
        }
    }
}