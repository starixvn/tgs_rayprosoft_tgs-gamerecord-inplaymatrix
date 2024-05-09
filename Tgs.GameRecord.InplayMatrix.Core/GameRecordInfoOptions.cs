namespace Tgs.GameRecord.InplayMatrix.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public IEnumerable<string> GameProviderCodes { get; init; } = default!;
	public string HangfirePrefix { get; set; } = "gamerecord-im-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-im-locking:";
	public double IntervalMinutes { get; init; } = 10;
	public double IntervalMinutesMax { get; init; } = 1440;
	public double IntervalMinutesMinimum { get; init; } = 5;
}
