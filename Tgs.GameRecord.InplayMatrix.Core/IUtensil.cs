using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IUtensil
{
    ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default);

    string GetESportsGameProviderCode();

    ValueTask<Guid> GetESportsGameProviderIdAsync(
        CancellationToken cancellationToken = default);

    ValueTask<int> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default);

    ValueTask<PlayerInfo> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
