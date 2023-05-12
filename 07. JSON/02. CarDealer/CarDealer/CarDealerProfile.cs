using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            // Supplier

            CreateMap<ImportSuppliers, Supplier>();

            // Part

            CreateMap<ImportParts, Part>();

            // Car

            CreateMap<ImportCar, Car>();

            // Customer

            CreateMap<ImportCustomer, Customer>();

            // Sale

            CreateMap<ImportSales, Sale>();
        }
    }
}
