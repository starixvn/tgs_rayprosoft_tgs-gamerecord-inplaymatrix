namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IScheduleFormByESportsDispatcher
{
    ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default);
}
