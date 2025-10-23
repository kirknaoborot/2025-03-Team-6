using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.IRepositories;
using ChannelSettings.Core.IServices;
using ChannelSettings.Core.Models;
using ChannelSettings.Domain.Entities;
using Infrastructure.Shared.Contracts;
using Infrastructure.Shared.Enums;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ChannelSettings.Application.Services
{
	public class ChannelService : IChannelService
	{
		private readonly IMapper _mapper;
		private readonly IChannelRepository _channelRepository;
		private readonly IBus _bus;
        private readonly ILogger<ChannelService> _logger;

        public ChannelService(IMapper mapper, IChannelRepository channelRepository, IBus bus, ILogger<ChannelService> logger)
		{
			_mapper = mapper;
			_channelRepository = channelRepository;
			_bus = bus;
            _logger = logger;
        }

		public async Task<ChannelModel> GetByIdAsync(int id, CancellationToken cancellationToken)
		{
			var channel = await _channelRepository.GetAsync(id, CancellationToken.None);
            _logger.LogInformation($"Найден канал: {channel.Name}");
            return _mapper.Map<Channel, ChannelModel>(channel);
		}

		public async Task<int> CreateAsync(CreatingChannel creatingChannel)
		{
			var channel = _mapper.Map<CreatingChannel, Channel>(creatingChannel);
			var creating = await _channelRepository.AddAsync(channel);
			await _channelRepository.SaveChangesAsync();

			var channelInfo = new ChannelEvent
			{
				Id = channel.Id,
				Name = channel.Name,
				Token = channel.Token,
				Type = channel.Type,
				Action = ChannelInfoAction.Create,
			};

			await _bus.Publish(channelInfo);
            _logger.LogInformation($"Создан новый канал: {channel.Name}");
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
            _logger.LogInformation($"Обновлен канал: {channel.Id}");

            var channelInfo = new ChannelEvent
			{
				Id = channel.Id,
				Name = channel.Name,
				Token = channel.Token,
				Type = channel.Type,
				Action = ChannelInfoAction.Update,
			};

			await _bus.Publish(channelInfo);
		}

		public async Task DeleteAsync(int id)
		{
			var channel = await _channelRepository.GetAsync(id, CancellationToken.None) ?? throw new Exception($"Запись с ID {id} не найдена");
			_channelRepository.Delete(channel);
			await _channelRepository.SaveChangesAsync();
            _logger.LogInformation($"Удален канал: {channel.Name}");

            var channelInfo = new ChannelEvent
			{
				Id = channel.Id,
				Name = channel.Name,
				Token = channel.Token,
				Type = channel.Type,
				Action = ChannelInfoAction.Delete,
			};

			await _bus.Publish(channelInfo);
		}

		public async Task<ICollection<ChannelModel>> GetAllAsync(CancellationToken cancellationToken)
		{
			ICollection<Channel> entities = await _channelRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation($"Получено каналов: {entities.Count}");
            return _mapper.Map<ICollection<Channel>, ICollection<ChannelModel>>(entities);
		}
	}
}
