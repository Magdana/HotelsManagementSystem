using HMS.Application.DTOs.Hotel;
using HMS.Domain.Entities;
using Mapster;

namespace HMS.Application.Mapping;

public class MappingConfig
{
    public static void RegisterMappings(TypeAdapterConfig config)
    {
        config.NewConfig<CreateHotelDto, Hotel>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Rating, src => src.Rating)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.Address, src => src.Address);

        config.NewConfig<Hotel, HotelResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Rating, src => src.Rating)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.Address, src => src.Address);

    }
}
