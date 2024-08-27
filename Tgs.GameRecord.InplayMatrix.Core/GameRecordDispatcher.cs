namespace Tgs.GameRecord.InplayMatrix.Core;

internal class GameRecordDispatcher(
	IPullRecordByIMESportsDispatcher iMESportsDispatcher) : IGameRecordDispatcher
{
	public ValueTask ExecuteAsync(GameRecordCommand command, CancellationToken cancellationToken = default)
		=> command.GameProviderCode.ToUpper() switch
		{
			"IM_ESP" => iMESportsDispatcher.PushCommandAsync(
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
