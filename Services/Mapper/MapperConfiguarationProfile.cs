using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Vaccines;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Requests.Manufacturer;
using ModelViews.Requests.Vaccine;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.Auth;
using ModelViews.Responses.ChildrenProfile;
using ModelViews.Responses.Manufacturer;
using ModelViews.Responses.Vaccine;
using ModelViews.Responses.VaccineBatch;
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

            // VaccineCategory
            CreateMap<VaccineCategoryRequestDTO, VaccineCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccineCategory, VaccineCategoryResponseDTO>();

            //Children Profile
            CreateMap<ChildrenProfileCreateUpdateDTO, ChildrenProfile>()
                            .ForMember(dest => dest.Id, opt => opt.Ignore())
                            .ForMember(dest => dest.AccountId, opt => opt.Ignore());
            CreateMap<ChildrenProfile, ChildrenProfileResponseDTO>();

            // VaccineBatch
            CreateMap<AddVaccineBatchRequestDTO, VaccineBatch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccineBatch, VaccineBatchResponseDTO>();

        }
    }
}
