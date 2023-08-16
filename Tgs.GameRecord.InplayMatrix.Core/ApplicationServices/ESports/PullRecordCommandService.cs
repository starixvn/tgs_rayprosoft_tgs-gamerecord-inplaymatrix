using Microsoft.Extensions.Logging;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly IGameInterfaceInplayMatrixClient _client;
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        IGameInterfaceInplayMatrixClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        int page = 1;

        while (true)
        {
            var gameRecords = await _client
                .GameRecordByIMESportsAsync(
                    reportUrl: command.ReportUrl,
                    merchantCode: command.MerchantCode,
                    merchantPrefix: command.MerchantPrefix,
                    startDate: command.StartDate,
                    endDate: command.EndDate,
                    page: page,
                    cancellationToken: cancellationToken)
                .ToArrayAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? Enumerable.Empty<GameRecordByIMESportsInfo>();

            _logger.LogDebug("LogOn:{logOn} Message: GameRecord Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

            if (!gameRecords.Any())
            {
                break;
            }

            page++;

            foreach (var gameRecord in gameRecords)
            {
                _logger.LogDebug("LogOn:{logOn} Message: GameRecord. GameRecords:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

                try
                {
                    await _service
                        .ExecuteAsync(
                        command: new(gameRecord),
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
                }
            }
        }
    }
}
