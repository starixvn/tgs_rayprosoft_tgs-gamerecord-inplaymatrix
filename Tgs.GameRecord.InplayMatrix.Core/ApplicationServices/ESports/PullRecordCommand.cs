namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

public record PullRecordCommand(
    Uri ReportUrl,
    string MerchantCode,
    string MerchantPrefix,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
