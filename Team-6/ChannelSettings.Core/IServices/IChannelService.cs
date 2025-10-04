using ChannelSettings.Core.Core;
using ChannelSettings.Core.Models;

namespace ChannelSettings.Core.IServices
{
    public interface IChannelService
    {
        Task<int> CreateAsync(CreatingChannel creatingChannel);
        Task DeleteAsync(int id);
        Task<ICollection<ChannelModel>> GetAllAsync(CancellationToken cancellationToken);
        Task<ChannelModel> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(int id, UpdatingChannel updatingChannel);
    }
}