using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.IRepositories;
using ChannelSettings.Core.IServices;
using ChannelSettings.Core.Models;
using ChannelSettings.Domain.Entities;

namespace ChannelSettings.Application.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IMapper _mapper;
        private readonly IChannelRepository _channelRepository;

        public ChannelService(IMapper mapper, IChannelRepository channelRepository)
        {
            _mapper = mapper;
            _channelRepository = channelRepository;
        }

        public async Task<ChannelModel> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var channel = await _channelRepository.GetAsync(id, CancellationToken.None);
            return _mapper.Map<Channel, ChannelModel>(channel);
        }

        public async Task<int> CreateAsync(CreatingChannel creatingChannel)
        {
            var channel = _mapper.Map<CreatingChannel, Channel>(creatingChannel);
            var creating = await _channelRepository.AddAsync(channel);
            await _channelRepository.SaveChangesAsync();
            return creating.Id;
        }

        public async Task UpdateAsync(int id, UpdatingChannel updatingChannel)
        {
            var channel = await _channelRepository.GetAsync(id, CancellationToken.None) ?? throw new Exception($"Запись с ID {id} не найдена");
            channel.Name = updatingChannel.Name;
            channel.Token = updatingChannel.Token;
            channel.Type = updatingChannel.Type;
            _channelRepository.Update(channel);
            await _channelRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var channel = await _channelRepository.GetAsync(id, CancellationToken.None) ?? throw new Exception($"Запись с ID {id} не найдена");
            _channelRepository.Delete(channel);
            await _channelRepository.SaveChangesAsync();
        }

        public async Task<ICollection<ChannelModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            ICollection<Channel> entities = await _channelRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<ICollection<Channel>, ICollection<ChannelModel>>(entities);
        }
    }
}
