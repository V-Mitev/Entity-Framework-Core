using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            // Supplier

            CreateMap<ImportSupplierDto, Supplier>();

            CreateMap<Supplier, ExportLocalSupplier>();

            // Part

            CreateMap<ImportPartDto, Part>()
                .ForMember(d => d.SupplierId,
                    opt => opt.MapFrom(s => s.SupplierId!.Value));

            CreateMap<Part, ExportPart>();

            // Car

            CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());

            CreateMap<Car, ExportCarWithDistance>();

            CreateMap<Car, ExportCarWithModelBmw>();

            CreateMap<Car, ExportCarWithTheirListOfParts>()
                 .ForMember(d => d.Parts,
                     opt => opt.MapFrom(s =>
                         s.PartsCars
                             .Select(pc => pc.Part)
                             .OrderByDescending(p => p.Price)
                             .ToArray()));

            // PartCar

            CreateMap<ImportCarPartDto, PartCar>();

            // Customer

            CreateMap<ImportCustomerDto, Customer>()
                .ForMember(d => d.BirthDate,
                opt => opt.MapFrom(s => DateTime.Parse(s.BirthDate, CultureInfo.InvariantCulture)));

            CreateMap<Customer, ExportTotalSalesByCustomer>()
                .ForMember(d => d.BoughtCars,
                opt => opt.MapFrom(s => s.Sales.Count));

            // Sale

            CreateMap<ImportSaleDto, Sale>();
        }
    }
}