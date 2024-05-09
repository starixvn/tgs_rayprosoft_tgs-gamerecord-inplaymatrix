namespace Tgs.GameRecord.InplayMatrix.Core;

public record GameRecordCommand(
	string GameProviderCode,
	Uri ReportUrl,
	string MerchantCode,
	string MerchantPrefix,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
