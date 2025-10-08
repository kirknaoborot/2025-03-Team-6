using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.Models;
using ChannelSettings.Domain.Entities;

namespace ChannelSettings.Core.MappingModels
{
    public class ChannelModelMapper : Profile
    {
        public ChannelModelMapper()
        {
            CreateMap<Channel, ChannelModel>();

            CreateMap<CreatingChannel, Channel>()
                .ForMember(d => d.Id, map => map.Ignore());

            CreateMap<UpdatingChannel, Channel>()
                .ForMember(d => d.Id, map => map.Ignore());
        }
    }
}
