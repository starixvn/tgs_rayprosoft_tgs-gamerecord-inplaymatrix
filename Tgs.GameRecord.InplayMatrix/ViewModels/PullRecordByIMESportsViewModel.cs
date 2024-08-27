using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.InplayMatrix.ViewModels;

public record PullRecordByIMESportsViewModel
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "http://operatorapi-report.staging.imaegisapi.com/")]
	public Uri ReportUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "OfL5H2hKOIP9tHp9ZIoqYeggJtFC39zQ")]
	public string MerchantCode { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "IM1S3")]
	public string MerchantPrefix { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-04-27T13:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2023-04-27T14:00:00+08:00")]
	public DateTimeOffset EndTime { get; init; }
}
