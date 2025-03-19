using System;

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

        // Manufacturer (ví dụ)
        CreateMap<ManufacturerRequestDTO, Manufacturer>();
        CreateMap<Manufacturer, ManufacturerResponseDTO>();
    }
}
