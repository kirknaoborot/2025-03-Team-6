using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.Models;
using ChannelSettings.DTO;

namespace ChannelSettings.MappingModel
{
    public class ChannelDtoMapper : Profile
    {
        public ChannelDtoMapper()
        {
            CreateMap<ChannelModel, ChannelDto>();

            CreateMap<CreatingChannelDto, CreatingChannel>();

            CreateMap<UpdatingChannelDto, UpdatingChannel>();
        }
    }
}
