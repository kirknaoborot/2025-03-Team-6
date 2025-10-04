using ChannelSettings.Core.IRepositories;
using ChannelSettings.Domain.Entities;

namespace ChannelSettings.DataAccess.Repositories
{
    public class ChannelRepository : Repository<Channel, int>, IChannelRepository
    {
        public ChannelRepository(ApplicationDbContext context) : base(context) { }
    }
}
