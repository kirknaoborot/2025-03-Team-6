using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.Models;
using ChannelSettings.DTO;
using Infrastructure.Shared.Enums;

namespace ChannelSettings.MappingModel
{
    public class ChannelDtoMapper : Profile
    {
        public ChannelDtoMapper()
        {
            CreateMap<ChannelModel, ChannelDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<CreatingChannelDto, CreatingChannel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<ChannelType>(src.Type)));

            CreateMap<UpdatingChannelDto, UpdatingChannel>()
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<ChannelType>(src.Type)));

        }
    }
}
