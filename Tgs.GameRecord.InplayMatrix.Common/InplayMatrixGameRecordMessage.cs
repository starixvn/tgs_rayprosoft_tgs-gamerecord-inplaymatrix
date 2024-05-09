using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.InplayMatrix.Common;

public record InplayMatrixGameRecordMessage
{
	[Required]
	public string GameProviderCode { get; init; } = default!;
	[Required]
	[DataType(DataType.Url)]
	public Uri ReportUrl { get; init; } = default!;
	[Required]
	public string MerchantCode { get; init; } = default!;
	[Required]
	public string MerchantPrefix { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; }
}
