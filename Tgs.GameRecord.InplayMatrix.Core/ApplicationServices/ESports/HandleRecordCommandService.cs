using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;

    public HandleRecordCommandService(
        IGameRecordHandleClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var status = GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = (DateTimeOffset?)null;
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;

        if (command.Record.IsSettled)
        {
            externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
            status = GameRecordStatus.Closed;
            settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
            commission = 0;
            turnover = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Record.WinLoss.ToDecimal().GetValueOrDefault(0);
            clientStatus = command.Record.IsCancelled ? GameRecordClientStatus.Void : winLoss <= 0 ? GameRecordClientStatus.Loss : GameRecordClientStatus.Win;
        }

        await _client
        .MessageAsync(
            gameProviderCode: "IM_ESP",
            externalRoundId: command.Record.ExternalRoundId,
            externalGameId: command.Record.ExternalGameId,
            playerCode: command.Record.PlayerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: winLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);
    }
}
