using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //string inputJson = File.ReadAllText(@"../../../Datasets/sales.json");

            string result = GetCarsWithTheirListOfParts(context);

            Console.WriteLine(result);
        }

        // Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportSuppliers[] supplierDtos =
                JsonConvert.DeserializeObject<ImportSuppliers[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

            foreach (ImportSuppliers supplierDto in supplierDtos)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }

        // Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportParts[] importPartsDto = JsonConvert.DeserializeObject<ImportParts[]>(inputJson);

            ICollection<Part> validParts = new HashSet<Part>();

            foreach (ImportParts partsDto in importPartsDto)
            {
                var supplier = context.Suppliers
                    .FirstOrDefault(s => s.Id == partsDto.SupplierId);

                if (supplier == null)
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partsDto);

                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        // Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsAndPartsDTO = JsonConvert.DeserializeObject<List<ImportCar>>(inputJson);

            List<PartCar> parts = new List<PartCar>();
            List<Car> cars = new List<Car>();

            foreach (var dto in carsAndPartsDTO)
            {
                Car car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TravelledDistance
                };
                cars.Add(car);

                foreach (var part in dto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = part,
                    };
                    parts.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}.";
        }

        // Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportCustomer[] customersDto =
                JsonConvert.DeserializeObject<ImportCustomer[]>(inputJson);

            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (ImportCustomer customerDto in customersDto)
            {
                Customer customer = mapper.Map<Customer>(customerDto);

                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        // Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportSales[] salesDto = JsonConvert.DeserializeObject<ImportSales[]>(inputJson);

            ICollection<Sale> sales = new HashSet<Sale>();

            foreach (ImportSales saleDto in salesDto)
            {
                Sale sale = mapper.Map<Sale>(saleDto);

                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        // Problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var orderedCustomers = context.Customers
               .OrderBy(c => c.BirthDate)
               .ThenBy(c => c.IsYoungDriver)
               .Select(c => new
               {
                   c.Name,
                   BirthDate = c.BirthDate.ToString(@"dd/MM/yyyy", CultureInfo.InvariantCulture),
                   c.IsYoungDriver
               })
               .AsNoTracking()
               .ToArray();

            string result = JsonConvert.SerializeObject(orderedCustomers, Formatting.Indented);

            return result;
        }

        // Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        // Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }

        // Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                        .Select(p => new
                        {
                            p.Part.Name,
                            Price = $"{p.Part.Price:f2}"
                        })
                })
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        // Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerSales = context.Customers
               .Where(c => c.Sales.Any())
               .Select(c => new
               {
                   fullName = c.Name,
                   boughtCars = c.Sales.Count(),
                   salePrices = c.Sales.SelectMany(x => x.Car.PartsCars.Select(x => x.Part.Price))
               })
               .ToArray();

            var totalSalesByCustomer = customerSales.Select(t => new
            {
                t.fullName,
                t.boughtCars,
                spentMoney = t.salePrices.Sum()
            })
            .OrderByDescending(t => t.spentMoney)
            .ThenByDescending(t => t.boughtCars)
            .ToArray();

            string result = JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);

            return result;
        }

        // Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesWithDiscount = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartsCars.Sum(p => p.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - s.Discount / 100):f2}"
                })
                .ToArray();

            string result = JsonConvert.SerializeObject(salesWithDiscount, Formatting.Indented);

            return result;
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