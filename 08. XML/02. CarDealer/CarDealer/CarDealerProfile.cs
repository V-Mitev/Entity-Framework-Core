using AutoMapper;
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

            // Part

            CreateMap<ImportPartDto, Part>()
                .ForMember(d => d.SupplierId,
                    opt => opt.MapFrom(s => s.SupplierId!.Value));

            // Car

            CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());

            // PartCar

            CreateMap<ImportCarPartDto, PartCar>();

            // Customer

            CreateMap<ImportCustomerDto, Customer>()
                .ForMember(d => d.BirthDate,
                opt => opt.MapFrom(s => DateTime.Parse(s.BirthDate, CultureInfo.InvariantCulture)));

            // Sale

            CreateMap<ImportSaleDto, Sale>();
        }
    }
}