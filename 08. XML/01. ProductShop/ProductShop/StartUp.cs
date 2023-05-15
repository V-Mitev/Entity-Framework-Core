using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //string inputXml = File.ReadAllText(@"../../../Datasets/categories-products.xml");

            string result = GetCategoriesByProductsCount(context);

            Console.WriteLine(result);
        }

        // Problem 01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportUserDto[] dto = xmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

            ICollection<User> users = new HashSet<User>();

            foreach (var importUserDto in dto)
            {
                User user = mapper.Map<User>(importUserDto);

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        // Problem 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportProductDto[] importProductDtos = 
                xmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (ImportProductDto productDto in importProductDtos)
            {
                Product product = mapper.Map<Product>(productDto);

                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        // Problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryDto[] importCategoryDtos = 
                xmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (ImportCategoryDto categoryDto in importCategoryDtos)
            {
                if (string.IsNullOrEmpty(categoryDto.Name))
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);

                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        // Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryAndProductDto[] importCategoryAndProductDtos = xmlHelper.Deserialize<ImportCategoryAndProductDto[]>(inputXml, "CategoryProducts");

            ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

            foreach (ImportCategoryAndProductDto cpDto in importCategoryAndProductDtos)
            {
                if (!context.Products.Any(p => p.Id == cpDto.ProductId) ||
                    !context.Categories.Any(c => c.Id == cpDto.CategoryId))
                {
                    continue;
                }

                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(cpDto);

                validCategoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCategoryProducts); 
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        // Problem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRange>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize(productsInRange, "Products");
        }

        // Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var userSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .AsNoTracking()
                .ProjectTo<ExportUserSoldProducts>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize(userSoldProducts, "Users");
        }

        // Problem 07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var categories = context.Categories
                .ProjectTo<ExportCategoriesByProductsCount>(mapper.ConfigurationProvider)
                .OrderByDescending(cp => cp.Count)
                .ThenBy(cp => cp.TotalRevenue)
                .AsNoTracking()
                .ToArray();

            return xmlHelper.Serialize(categories, "Categories");
        }

        // Problem 08


        private static IMapper InitializeAutoMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
        }
    }
}