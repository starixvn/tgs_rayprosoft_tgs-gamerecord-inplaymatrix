using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.InplayMatrix.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.InplayMatrix.Core.JobApplicationServices;

internal class JobExecuteCommandService(
    ILogger<JobExecuteCommandService> logger,
    IOptions<GameRecordInfoOptions> options,
    IMessageSender messageSender) : ICommandService<JobExecuteCommand>
{
    private readonly double _intervalDays = options.Value.IntervalMinutesMax < options.Value.IntervalMinutes
        ? options.Value.IntervalMinutesMax
        : options.Value.IntervalMinutesMinimum > options.Value.IntervalMinutes
        ? options.Value.IntervalMinutesMinimum
        : options.Value.IntervalMinutes;

    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\T00:00:00K", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
        var startTime = endTime.AddMinutes(_intervalDays * -1);

        var parameters = new InplayMatrixGameRecordMessage
        {
            GameProviderCode = command.GameProviderCode,
            ReportUrl = command.ReportUrl,
            MerchantCode = command.MerchantCode,
            MerchantPrefix = command.MerchantPrefix,
            StartTime = startTime,
            EndTime = endTime
        };

        logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

        return messageSender.PublishAsync(
            subject: InplayMatrixNatsConst.SchedulePublish,
            data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(parameters)),
            cancellationToken: cancellationToken);
    }
}
