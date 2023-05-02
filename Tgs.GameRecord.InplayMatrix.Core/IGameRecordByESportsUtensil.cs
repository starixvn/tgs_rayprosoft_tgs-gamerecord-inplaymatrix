using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameRecord.InplayMatrix.Core.Models;

namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IGameRecordByESportsUtensil
{
    public ValueTask<RecordByIMESportsInfo> GameRecordConvertRecordAsync(
        GameRecordByIMESportsInfo source,
        CancellationToken cancellationToken = default);
}
