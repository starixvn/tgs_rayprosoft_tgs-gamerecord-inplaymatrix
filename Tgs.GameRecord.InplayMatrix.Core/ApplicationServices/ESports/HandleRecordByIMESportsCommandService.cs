using System.Text.Json.Nodes;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

internal class HandleRecordByIMESportsCommandService(
    IGameRecordHandleClient client) : ICommandService<HandleRecordByIMESportsCommand>
{
    public async ValueTask ExecuteAsync(HandleRecordByIMESportsCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = WagerStates.Running;
        var externalCreatedOn = command.Record.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;

        if (command.Record.IsSettled)
        {
            externalClosedOn = command.Record.ExternalClosedOn.ToDateTimeOffset();
            settle = command.Record.Settle.ToDecimal().GetValueOrDefault(0);
            commission = 0;
            turnover = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Record.WinLoss.ToDecimal().GetValueOrDefault(0);
            wagerState = command.Record.IsCancelled ? WagerStates.Void : winLoss <= 0 ? WagerStates.Lose : WagerStates.Win;
        }

        await client.MessageAsync(
            gameProviderCode: "IM_ESP",
            playerCode: command.Record.PlayerCode,
            wagerState: wagerState,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: command.Record.ExternalGameId,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: winLoss,
            additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);
    }
}
