using Microsoft.Extensions.Logging;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

internal class ManualPullRecordCommandService : ICommandService<ManualPullRecordCommand>
{
    private readonly IGameInterfaceInplayMatrixClient _client;
    private readonly IGameRecordByESportsUtensil _gameRecordUtensil;
    private readonly ILogger<ManualPullRecordCommandService> _logger;
    private readonly ICommandService<ManualHandleRecordCommand> _service;

    public ManualPullRecordCommandService(
        ILogger<ManualPullRecordCommandService> logger,
        IGameInterfaceInplayMatrixClient client,
        IGameRecordByESportsUtensil gameRecordUtensil,
        ICommandService<ManualHandleRecordCommand> service)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default)
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
                    var record = await _gameRecordUtensil
                        .GameRecordConvertRecordAsync(
                            gameRecord,
                            cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("LogOn:{logOn} Message: GameRecord. Record:{record}", DateTime.Now.ToString("o"), record);

                    await _service
                        .ExecuteAsync(
                            command: new(record),
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogOn:{logOn} Message: GameRecord Exception. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);
                }
            }
        }
    }
}
