namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

public record PullRecordByIMESportsCommand(
	Uri ReportUrl,
	string MerchantCode,
	string MerchantPrefix,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
