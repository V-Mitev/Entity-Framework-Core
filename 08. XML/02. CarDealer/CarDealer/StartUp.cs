using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //string inputXml = File.ReadAllText(@"../../../Datasets/sales.xml");

            string result = GetCarsWithTheirListOfParts(context);

            Console.WriteLine(result);
        }

        // Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportSupplierDto[] suppliers = 
                xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>(); 

            foreach (ImportSupplierDto supplierDto in suppliers)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}";
        }

        // Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportPartDto[] partDtos = 
                xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");

            ICollection<Part> validParts = new HashSet<Part>();

            foreach (ImportPartDto partDto in partDtos)
            {
                if (!partDto.SupplierId.HasValue ||
                    !context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDto);

                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        // Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportCarDto[] importCarDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");

            ICollection<Car> validCars = new HashSet<Car>();

            ICollection<PartCar> validParts = new HashSet<PartCar>();

            foreach (ImportCarDto carDto in importCarDtos)
            {
                Car car = mapper.Map<Car>(carDto);

                foreach (var partDto in carDto.Parts.DistinctBy(p => p.PartId))
                {
                    if (!context.Parts.Any(p => p.Id == partDto.PartId))
                    {
                        continue;
                    }

                    PartCar part = new PartCar()
                    {
                        PartId = partDto.PartId
                    };

                    car.PartsCars.Add(part);
                }

                validCars.Add(car);
            }

            context.Cars.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}";
        }

        // Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportCustomerDto[] importCustomerDtos = 
                xmlHelper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (ImportCustomerDto customerDto in importCustomerDtos)
            {
                Customer customer = mapper.Map<Customer>(customerDto);

                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        // Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ImportSaleDto[] importSaleDtos = 
                xmlHelper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");

            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach (ImportSaleDto saleDto in importSaleDtos)
            {
                if (!context.Cars.Any(c => c.Id == saleDto.CarId))
                {
                    continue;
                }

                Sale sale = mapper.Map<Sale>(saleDto);

                validSales.Add(sale);
            }

            context.Sales.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}";
        }

        // Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarWithDistance>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            var result = xmlHelper.Serialize<ExportCarWithDistance[]>(cars, "cars");

            return result;
        }

        // Problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var cars = context.Cars
                .Where(c => c.Make.ToUpper() == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarWithModelBmw>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            string result = xmlHelper.Serialize<ExportCarWithModelBmw[]>(cars, "cars");

            return result;
        }

        // Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportLocalSupplier>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            string result = xmlHelper.Serialize<ExportLocalSupplier[]>(suppliers, "suppliers");

            return result;
        }

        // Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            IMapper mapper = CreateMapper();

            XmlHelper xmlHelper = new XmlHelper();

            ExportCarWithTheirListOfParts[] carsWithParts = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithTheirListOfParts>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize(carsWithParts, "cars");
        }

        // Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var customerSales = context.Customers
            .Where(cus => cus.Sales.Any())
            .Select(cus => new
            {
                fullName = cus.Name,
                boughtCars = cus.Sales.Count(),
                moneyCars = cus.IsYoungDriver
                    ? cus.Sales.SelectMany(s => s.Car.PartsCars.Select(p => Math.Round(p.Part.Price * 0.95m, 2)))
                    : cus.Sales.SelectMany(s => s.Car.PartsCars.Select(p => Math.Round(p.Part.Price, 2)))
            })
            .ToArray();

            var output = customerSales
                .Select(o => new ExportTotalSalesByCustomerDto()
                {
                    FullName = o.fullName,
                    BoughtCars = o.boughtCars,
                    SpentMoney = o.moneyCars.Sum()
                })
                .OrderByDescending(o => o.SpentMoney)
                .ToArray();

            return xmlHelper.Serialize<ExportTotalSalesByCustomerDto[]>(output, "customers");
        }

        // 19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var xmlHelper = new XmlHelper();

            var sales = context.Sales
                .Select(s => new ExportSalesWithAppliedDiscountDto()
                {
                    CarDtoSales = new CarDtoSales()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },

                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(p => p.Part.Price).ToString("0.00"),
                    PriceWithDiscount = Math.Round((double)(s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - (s.Discount / 100))), 4)
                })
                .ToArray();

            return xmlHelper.Serialize<ExportSalesWithAppliedDiscountDto[]>(sales, "sales");
        }

        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
        }
    }
}