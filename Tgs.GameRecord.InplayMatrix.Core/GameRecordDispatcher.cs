using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core;

internal class GameRecordDispatcher(
	ICommandService<PullRecordByIMESportsCommand> imESportsService) : IGameRecordDispatcher
{
	public ValueTask ExecuteAsync(GameRecordCommand command, CancellationToken cancellationToken = default)
		=> command.GameProviderCode.ToUpper() switch
		{
			"IM_ESP" => imESportsService.ExecuteAsync(
				command: new(
					ReportUrl: command.ReportUrl,
					MerchantCode: command.MerchantCode,
					MerchantPrefix: command.MerchantPrefix,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			_ => throw new NotSupportedException()
		};
}
