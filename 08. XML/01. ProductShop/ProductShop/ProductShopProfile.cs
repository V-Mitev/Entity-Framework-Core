using AutoMapper;
using ProductShop.DTOs.Export;
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
            CreateMap<User, ExportUserSoldProducts>()
                .ForMember(d => d.SoldProducts,
                opt => opt.MapFrom(s => s.ProductsSold));

            // Product

            CreateMap<ImportProductDto, Product>();

            CreateMap<Product, ExportProductsInRange>()
                .ForMember(d => d.Buyer,
                opt => opt.MapFrom(opt => opt.Buyer.FirstName + " " + opt.Buyer.LastName));

            CreateMap<Product, ExportSoldProducts>();

            // Category

            CreateMap<ImportCategoryDto, Category>();

            CreateMap<Category, ExportCategoriesByProductsCount>()
                .ForMember(d => d.Count,
                opt => opt.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(d => d.AveragePrice,
                opt => opt.MapFrom(s => s.CategoryProducts.Average(p => p.Product.Price)))
                .ForMember(d => d.TotalRevenue,
                opt => opt.MapFrom(s => s.CategoryProducts.Sum(p => p.Product.Price)));

            //CategoryProduct

            CreateMap<ImportCategoryAndProductDto, CategoryProduct>();
        }
    }
}