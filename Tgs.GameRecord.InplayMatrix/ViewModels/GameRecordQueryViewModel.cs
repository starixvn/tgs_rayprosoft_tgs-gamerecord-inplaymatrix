namespace Tgs.GameRecord.InplayMatrix.ViewModels;

public record GameRecordQueryViewModel(
    Uri GameApiUrl,
    string CasinoId,
    string GameHistoryApiToken,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate);
