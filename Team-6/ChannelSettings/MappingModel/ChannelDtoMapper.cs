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
				.ForMember(src => src.Type, opt => opt.MapFrom(x => x.Type.ToString()));

            CreateMap<CreatingChannelDto, CreatingChannel>()
				.ForMember(src => src.Type, opt => opt.MapFrom(x => x.Type));

			CreateMap<UpdatingChannelDto, UpdatingChannel>()
				.ForMember(src => src.Type, opt => opt.MapFrom(x => x.Type));
		}
    }
}
