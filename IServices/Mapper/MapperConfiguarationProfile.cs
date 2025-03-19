using AutoMapper;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Vaccines;
using ModelViews.Requests.Manufacturer;
using ModelViews.Requests.Vaccine;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.Manufacturer;
using ModelViews.Responses.Vaccine;
using ModelViews.Responses.VaccineCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Mapper
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            // Vaccine
            CreateMap<VaccineRequestDTO, Vaccine>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<Vaccine, VaccineResponseDTO>()
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Batch.Manufacturer.Name))
                .ForMember(dest => dest.ManufacturerCountry, opt => opt.MapFrom(src => src.Batch.Manufacturer.CountryName));

            // VaccineCategory (ví dụ)
            CreateMap<VaccineCategoryRequestDTO, VaccineCategory>();
            CreateMap<VaccineCategory, VaccineCategoryResponseDTO>();



        }
    }
}
