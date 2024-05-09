namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IGameRecordDispatcher
{
	ValueTask ExecuteAsync(GameRecordCommand command, CancellationToken cancellationToken = default);
}
