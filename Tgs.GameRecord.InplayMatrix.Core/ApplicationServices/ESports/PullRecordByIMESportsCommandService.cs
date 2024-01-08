using Microsoft.Extensions.Logging;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

internal class PullRecordByIMESportsCommandService(
    ILogger<PullRecordByIMESportsCommandService> logger,
    IGameInterfaceInplayMatrixClient client,
    ICommandService<HandleRecordByIMESportsCommand> service) : ICommandService<PullRecordByIMESportsCommand>
{
    public async ValueTask ExecuteAsync(PullRecordByIMESportsCommand command, CancellationToken cancellationToken = default)
    {
        int page = 1;
        IEnumerable<GameRecordByIMESportsInfo> infos;

        do
        {
            infos = await client.GameRecordByIMESportsAsync(
                reportUrl: command.ReportUrl,
                merchantCode: command.MerchantCode,
                merchantPrefix: command.MerchantPrefix,
                startDate: command.StartTime,
                endDate: command.EndTime,
                page: page,
                cancellationToken: cancellationToken)
               .ToArrayAsync(
                cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? Enumerable.Empty<GameRecordByIMESportsInfo>();

            logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. GameRecords:{infos}", DateTime.Now.ToString("o"), infos);

            page++;

            foreach (var info in infos)
            {
                logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{info}", DateTime.Now.ToString("o"), info);

                try
                {
                    await service
                        .ExecuteAsync(
                        command: new(info),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({info})", DateTime.Now.ToString("o"), info);
                }
            }
        } while (infos.Any());
    }
}
