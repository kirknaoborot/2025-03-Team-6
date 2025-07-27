

using OperatorStatusService.Domain.Models;

namespace OperatorStatusService.Domain.Interfaces
{
    public interface IOperatorStatusService
    {
        Task<List<UserInfo>> GetActiveOperatorsAsync(CancellationToken cancellationToken);
    }
}