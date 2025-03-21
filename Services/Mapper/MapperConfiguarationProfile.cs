using AutoMapper;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Orders;
using IRepositories.Entity.Schedules;
using IRepositories.Entity.Vaccines;
using ModelViews.Requests.Auth;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Requests.History;
using ModelViews.Requests.Manufacturer;
using ModelViews.Requests.Order;
using ModelViews.Requests.Vaccine;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Requests.VaccineCenter;
using ModelViews.Requests.VaccineHistory;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.Auth;
using ModelViews.Responses.ChildrenProfile;
using ModelViews.Responses.Feedback;
using ModelViews.Responses.Manufacturer;
using ModelViews.Responses.Order;
using ModelViews.Responses.Vaccine;
using ModelViews.Responses.VaccineBatch;
using ModelViews.Responses.VaccineCategory;
using ModelViews.Responses.VaccineCenter;
using ModelViews.Responses.VaccineHistory;
using ModelViews.Responses.VaccinePackage;
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
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<Vaccine, VaccineResponseDTO>()
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Batch.Manufacturer.Name))
                .ForMember(dest => dest.ManufacturerCountry, opt => opt.MapFrom(src => src.Batch.Manufacturer.CountryName));

            // Vaccine Package
            CreateMap<VaccinePackageRequestDTO, VaccinePackage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PackageStatus, opt => opt.Ignore());
            CreateMap<VaccinePackage, VaccinePackageResponseDTO>()
                .ForMember(dest => dest.Vaccines, opt => opt.MapFrom(src => src.PackageDetails));
            CreateMap<VaccinePackageDetail, VaccineWithDoseResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Vaccine.Id))
                .ForMember(dest => dest.DoseNumber, opt => opt.MapFrom(src => src.doseNumber));
            CreateMap<VaccineDoseRequestDTO, VaccinePackageDetail>();
            CreateMap<VaccinePackageUpdateRequestDTO, VaccinePackageDetail>();

            // VaccinePackageDetail
            CreateMap<VaccinePackageDetailsRequestDTO, VaccinePackageDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccinePackageDetail, VaccinePackageDetailsResponseDTO>();

            // VaccineCategory
            CreateMap<VaccineCategoryRequestDTO, VaccineCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccineCategory, VaccineCategoryResponseDTO>();

            // Children Profile
            CreateMap<ChildrenProfileCreateUpdateDTO, ChildrenProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore());
            CreateMap<ChildrenProfile, ChildrenProfileResponseDTO>();

            // VaccineBatch
            CreateMap<AddVaccineBatchRequestDTO, VaccineBatch>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccineBatch, VaccineBatchResponseDTO>();

            // VaccineCenter
            CreateMap<VaccineCenterRequestDTO, VaccineCenter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<VaccineCenterUpdateDTO, VaccineCenter>();
            CreateMap<VaccineCenter, VaccineCenterResponseDTO>();

            // Manufacturer
            CreateMap<ManufacturerRequestDto, Manufacturer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Manufacturer, ManufacturerResponseDto>();

            // Account
            CreateMap<UpdateAccountRequestDTO, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
            CreateMap<Account, ProfileResponseDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? "Not provided"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "1"));

            // Order
            CreateMap<OrderRequestDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalOrderPrice, opt => opt.Ignore());
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VaccineDetails, opt => opt.MapFrom(src => src.OrderVaccineDetails))
                .ForMember(dest => dest.PackageDetails, opt => opt.MapFrom(src => src.OrderPackageDetails));

            // OrderVaccineDetails
            CreateMap<OrderVaccineDetails, OrderVaccineDetailResponseDTO>()
                .ForMember(dest => dest.OrderVaccineId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VaccineName, opt => opt.MapFrom(src => src.Vaccine.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Vaccine.Image));

            // OrderPackageDetails
            CreateMap<OrderPackageDetails, OrderPackageDetailResponseDTO>()
                .ForMember(dest => dest.OrderPackageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VaccinePackageName, opt => opt.MapFrom(src => src.VaccinePackage.PackageName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.VaccinePackage.PackageDescription));

            // Payment
            CreateMap<Payment, PaymentDetailsResponseDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id));

            // Feedback
            CreateMap<Feedback, FeedbackResponseDTO>();

            // VaccineHistory
            CreateMap<CreateVaccineHistoryRequestDTO, VaccineHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedStatus, opt => opt.Ignore());

            CreateMap<SendVaccineCertificateRequestDTO, VaccineHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AccountId, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentationProvided, opt => opt.Ignore()) 
                .ForMember(dest => dest.VaccinedStatus, opt => opt.Ignore())
                .ForMember(dest => dest.VaccineId, opt => opt.Ignore())
                .ForMember(dest => dest.CenterId, opt => opt.Ignore())
                .ForMember(dest => dest.AdministeredDate, opt => opt.Ignore())
                .ForMember(dest => dest.AdministeredBy, opt => opt.Ignore())
                .ForMember(dest => dest.DosedNumber, opt => opt.Ignore());

            CreateMap<VaccineHistory, VaccineHistoryResponseDTO>();
        }
    }
}
