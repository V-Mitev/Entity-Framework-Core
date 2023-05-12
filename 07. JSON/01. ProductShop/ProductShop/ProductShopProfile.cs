using AutoMapper;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            // Users 

            CreateMap<ImportUserDto, User>();

            // Products

            CreateMap<ImportProductDto, Product>();

            // Categories

            CreateMap<ImportCategoryDto, Category>();

            // CategoryProduct

            CreateMap<ImportCategoryProductDto, CategoryProduct>();
        }
    }
}
