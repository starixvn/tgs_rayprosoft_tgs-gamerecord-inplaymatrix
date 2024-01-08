using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IGameRecordDispatcher
{
    ValueTask ExecuteAsync(GameRecordCommand command, CancellationToken cancellationToken = default);
}
