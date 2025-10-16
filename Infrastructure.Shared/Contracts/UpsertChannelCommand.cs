namespace Infrastructure.Shared.Contracts
{
    public record UpsertChannelCommand(
        int ChannelSettingsId,
        string Name,
        string Token,
        string ChannelType,
        bool IsActive,
        DateTime UpdatedAt,
        Guid CorrelationId);
}

