using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.InplayMatrix.Common;
using Tgs.GameRecord.InplayMatrix.ViewModels;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.InplayMatrix.Controllers;

[ApiController]
[Route("im_esp")]
public class IMESPController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		PullRecordByIMESportsViewModel source)
		=> messageSender.PublishAsync(
			subject: InplayMatrixNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(new InplayMatrixGameRecordMessage
			{
				GameProviderCode = "IM_ESP",
				ReportUrl = source.ReportUrl,
				MerchantCode = source.MerchantCode,
				MerchantPrefix = source.MerchantPrefix,
				StartTime = source.StartTime,
				EndTime = source.EndTime
			}),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		PullRecordByIMESportsViewModel source)
		=> messageSender.PublishAsync(
			subject: InplayMatrixNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(new InplayMatrixGameRecordMessage
			{
				GameProviderCode = "IM_ESP",
				ReportUrl = source.ReportUrl,
				MerchantCode = source.MerchantCode,
				MerchantPrefix = source.MerchantPrefix,
				StartTime = source.StartTime,
				EndTime = source.EndTime
			}),
			cancellationToken: HttpContext.RequestAborted);
}
