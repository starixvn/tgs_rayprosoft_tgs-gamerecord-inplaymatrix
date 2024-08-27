using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IPullRecordByIMESportsDispatcher
{
	ValueTask PushCommandAsync(PullRecordByIMESportsCommand command, CancellationToken cancellationToken = default);
}
