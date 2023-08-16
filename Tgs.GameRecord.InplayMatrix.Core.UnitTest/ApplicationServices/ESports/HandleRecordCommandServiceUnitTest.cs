using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest.ApplicationServices.ESports;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process_Settlement_Loss()
    {
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordByIMESportsInfo
            {
                ExternalRoundId = "432368314",
                ExternalGameId = "ESPORTSBULL",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-04-27 10:17:30 +08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-04-27 14:12:34 +08:00").ToTimestamp(),
                IsSettled = true,
                IsResettled = false,
                IsCancelled = false,
                Bet = 5m,
                Settle = 0.0m,
                WinLoss = -5m,
                AdditionalData = "{\"Provider\":\"ESPORTSBULL_ESPORTS\",\"GameId\":\"ESPORTSBULL\",\"BetId\":\"432368314\",\"WagerCreationDateTime\":\"2023-04-27 10:17:30 +08:00\",\"LastUpdatedDate\":\"2023-04-27 14:12:34 +08:00\",\"PlayerId\":\"IM1S3_4ucmqplxb029\",\"ProviderPlayerId\":\"IM1S3IM1S3_4ucmqplxb029\",\"Currency\":\"CNY\",\"StakeAmount\":\"5.000000\",\"WinLoss\":\"-5.000000\",\"OddsType\":\"EURO\",\"TotalOdds\":\"1.093\",\"WagerType\":\"Single\",\"Platform\":\"Web\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsCancelled\":\"0\",\"SettlementDateTime\":\"2023-04-27 14:12:34 +08:00\",\"Tolerance\":\"255\",\"FlagCategory\":\"\",\"DetailItems\":[{\"EventName\":\"herO VS KeeN\",\"EventDateTime\":\"2023-04-27 17:30:00 +08:00\",\"CompetitionName\":\"Global StarCraft II League Season 1\",\"HomeTeamName\":\"herO\",\"AwayTeamName\":\"KeeN\",\"FavTeam\":\"\",\"Period\":\"\",\"HomeTeamHTScore\":\"0\",\"AwayTeamHTScore\":\"0\",\"HomeTeamFTScore\":\"0\",\"AwayTeamFTScore\":\"0\",\"FTScore\":[{\"Option\":\"herO\",\"Outcome\":0},{\"Option\":\"KeeN\",\"Outcome\":3}],\"WagerHomeTeamScore\":\"0\",\"WagerAwayTeamScore\":\"0\",\"MarketType\":\"\",\"Handicap\":\"0.00\",\"CancelReason\":\"\",\"SportsID\":53,\"SportsName\":\"STARCRAFT2\",\"BetType\":\"BO3 Series Win\",\"EventId\":\"12933529\",\"EventType\":\"BO3\",\"GameOrder\":\"0\",\"Selection\":\"herO\",\"Odds\":\"1.093\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsWagerItemCancelled\":\"0\",\"WagerItemCancelType\":\"0\",\"ResultStatus\":\"2\",\"BaseTierID\":\"0\",\"BaseTierCode\":\"0\",\"BaseTierName\":\"Other\"}]}"
            });

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
                gameProviderCode: Arg.Is("IM_ESP"),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                status: Arg.Is(GameRecordStatus.Closed),
                clientStatus: Arg.Is(GameRecordClientStatus.Loss),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Win()
    {
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordByIMESportsInfo
            {
                ExternalRoundId = "432368343",
                ExternalGameId = "ESPORTSBULL",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-04-27 15:54:50 +08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-04-27 15:59:47 +08:00").ToTimestamp(),
                IsSettled = true,
                IsResettled = false,
                IsCancelled = false,
                Bet = 5m,
                Settle = 8.005m,
                WinLoss = 3.005000m,
                AdditionalData = "{\"Provider\":\"ESPORTSBULL_ESPORTS\",\"GameId\":\"ESPORTSBULL\",\"BetId\":\"432368324\",\"WagerCreationDateTime\":\"2023-04-27 13:40:17 +08:00\",\"LastUpdatedDate\":\"2023-04-27 14:08:13 +08:00\",\"PlayerId\":\"IM1S3_4ucmqplxb029\",\"ProviderPlayerId\":\"IM1S3IM1S3_4ucmqplxb029\",\"Currency\":\"CNY\",\"StakeAmount\":\"5.000000\",\"WinLoss\":\"3.005000\",\"OddsType\":\"EURO\",\"TotalOdds\":\"1.601\",\"WagerType\":\"Single\",\"Platform\":\"Web\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsCancelled\":\"0\",\"SettlementDateTime\":\"2023-04-27 14:08:13 +08:00\",\"Tolerance\":\"255\",\"FlagCategory\":\"\",\"DetailItems\":[{\"EventName\":\"Team WE Academy VS Joy Dream\",\"EventDateTime\":\"2023-04-27 14:00:00 +08:00\",\"CompetitionName\":\"LDL 2023 Split 2\",\"HomeTeamName\":\"Team WE Academy\",\"AwayTeamName\":\"Joy Dream\",\"FavTeam\":\"\",\"Period\":\"\",\"HomeTeamHTScore\":\"0\",\"AwayTeamHTScore\":\"0\",\"HomeTeamFTScore\":\"0\",\"AwayTeamFTScore\":\"0\",\"FTScore\":[{\"Option\":\"Team WE Academy\",\"Outcome\":3},{\"Option\":\"Joy Dream\",\"Outcome\":0}],\"WagerHomeTeamScore\":\"0\",\"WagerAwayTeamScore\":\"0\",\"MarketType\":\"\",\"Handicap\":\"0.00\",\"CancelReason\":\"\",\"SportsID\":45,\"SportsName\":\"LEAGUE OF LEGENDS\",\"BetType\":\"BO3 Series Win\",\"EventId\":\"12954756\",\"EventType\":\"BO3\",\"GameOrder\":\"0\",\"Selection\":\"Team WE Academy\",\"Odds\":\"1.601\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsWagerItemCancelled\":\"0\",\"WagerItemCancelType\":\"0\",\"ResultStatus\":\"1\",\"BaseTierID\":\"2\",\"BaseTierCode\":\"2\",\"BaseTierName\":\"Tier2\"}]}"
            });

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
                gameProviderCode: "IM_ESP",
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                status: Arg.Is(GameRecordStatus.Closed),
                clientStatus: Arg.Is(GameRecordClientStatus.Win),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Unsettlement()
    {
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordByIMESportsInfo
            {
                ExternalRoundId = "432368314",
                ExternalGameId = "ESPORTSBULL",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-04-27 10:17:30 +08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
                IsSettled = false,
                IsResettled = false,
                IsCancelled = false,
                Bet = 5m,
                Settle = 0m,
                WinLoss = 0m,
                AdditionalData = "{\"Provider\":\"ESPORTSBULL_ESPORTS\",\"GameId\":\"ESPORTSBULL\",\"BetId\":\"432368314\",\"WagerCreationDateTime\":\"2023-04-27 10:17:30 +08:00\",\"LastUpdatedDate\":\"2023-04-27 10:17:30 +08:00\",\"PlayerId\":\"IM1S3_4ucmqplxb029\",\"ProviderPlayerId\":\"IM1S3IM1S3_4ucmqplxb029\",\"Currency\":\"CNY\",\"StakeAmount\":\"5.000000\",\"WinLoss\":\"0.000000\",\"OddsType\":\"EURO\",\"TotalOdds\":\"1.093\",\"WagerType\":\"Single\",\"Platform\":\"Web\",\"IsSettled\":\"0\",\"IsResettled\":\"0\",\"IsCancelled\":\"0\",\"SettlementDateTime\":\"\",\"Tolerance\":\"255\",\"FlagCategory\":\"\",\"DetailItems\":[{\"EventName\":\"herO VS KeeN\",\"EventDateTime\":\"2023-04-27 17:30:00 +08:00\",\"CompetitionName\":\"Global StarCraft II League Season 1\",\"HomeTeamName\":\"herO\",\"AwayTeamName\":\"KeeN\",\"FavTeam\":\"\",\"Period\":\"\",\"HomeTeamHTScore\":\"0\",\"AwayTeamHTScore\":\"0\",\"HomeTeamFTScore\":\"0\",\"AwayTeamFTScore\":\"0\",\"WagerHomeTeamScore\":\"0\",\"WagerAwayTeamScore\":\"0\",\"MarketType\":\"\",\"Handicap\":\"0.00\",\"CancelReason\":\"\",\"SportsID\":53,\"SportsName\":\"STARCRAFT2\",\"BetType\":\"BO3 Series Win\",\"EventId\":\"12933529\",\"EventType\":\"BO3\",\"GameOrder\":\"0\",\"Selection\":\"herO\",\"Odds\":\"1.093\",\"IsSettled\":\"0\",\"IsResettled\":\"0\",\"IsWagerItemCancelled\":\"0\",\"WagerItemCancelType\":\"0\",\"ResultStatus\":\"\",\"BaseTierID\":\"0\",\"BaseTierCode\":\"0\",\"BaseTierName\":\"Other\"}]}"
            });

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
                gameProviderCode: Arg.Is("IM_ESP"),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                status: Arg.Is(GameRecordStatus.Open),
                clientStatus: Arg.Is(GameRecordClientStatus.Open),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is((DateTimeOffset?)null),
                bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(0.0m),
                playerWinLoss: Arg.Is(0.0m),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Void()
    {
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GameRecordByIMESportsInfo
            {
                ExternalRoundId = "432368314",
                ExternalGameId = "ESPORTSBULL",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-04-27 15:54:50 +08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-04-27 15:59:47 +08:00").ToTimestamp(),
                IsSettled = true,
                IsResettled = false,
                IsCancelled = true,
                Bet = 5m,
                Settle = 0m,
                WinLoss = 0m,
                AdditionalData = "{\"Provider\":\"ESPORTSBULL_ESPORTS\",\"GameId\":\"ESPORTSBULL\",\"BetId\":\"432368343\",\"WagerCreationDateTime\":\"2023-04-27 15:54:50 +08:00\",\"LastUpdatedDate\":\"2023-04-27 15:59:47 +08:00\",\"PlayerId\":\"IM1S3_4ucmqplxb029\",\"ProviderPlayerId\":\"IM1S3IM1S3_4ucmqplxb029\",\"Currency\":\"CNY\",\"StakeAmount\":\"5.000000\",\"WinLoss\":\"0.000000\",\"OddsType\":\"EURO\",\"TotalOdds\":\"13.952\",\"WagerType\":\"Single\",\"Platform\":\"Web\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsCancelled\":\"1\",\"SettlementDateTime\":\"2023-04-27 15:59:47 +08:00\",\"Tolerance\":\"0\",\"FlagCategory\":\"\",\"DetailItems\":[{\"EventName\":\"eSuba VS Team GO\",\"EventDateTime\":\"2023-04-29 00:30:00 +08:00\",\"CompetitionName\":\"EMEA Masters Spring 2023\",\"HomeTeamName\":\"eSuba\",\"AwayTeamName\":\"Team GO\",\"FavTeam\":\"\",\"Period\":\"\",\"HomeTeamHTScore\":\"0\",\"AwayTeamHTScore\":\"0\",\"HomeTeamFTScore\":\"0\",\"AwayTeamFTScore\":\"0\",\"WagerHomeTeamScore\":\"0\",\"WagerAwayTeamScore\":\"0\",\"MarketType\":\"\",\"Handicap\":\"0.00\",\"CancelReason\":\"\",\"SportsID\":45,\"SportsName\":\"LEAGUE OF LEGENDS\",\"BetType\":\"Game 1 Win (Live Odds)\",\"EventId\":\"12929913\",\"EventType\":\"BO7\",\"GameOrder\":\"1\",\"Selection\":\"eSuba\",\"Odds\":\"13.952\",\"IsSettled\":\"1\",\"IsResettled\":\"0\",\"IsWagerItemCancelled\":\"1\",\"WagerItemCancelType\":\"2\",\"ResultStatus\":\"\",\"BaseTierID\":\"1\",\"BaseTierCode\":\"1\",\"BaseTierName\":\"Tier1\"}]}"
            });

        var service = new HandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .MessageAsync(
                gameProviderCode: Arg.Is("IM_ESP"),
                externalRoundId: Arg.Is(command.Record.ExternalRoundId),
                externalGameId: Arg.Is(command.Record.ExternalGameId),
                playerCode: Arg.Is(command.Record.PlayerCode),
                status: Arg.Is(GameRecordStatus.Closed),
                clientStatus: Arg.Is(GameRecordClientStatus.Void),
                externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
                externalClosedOn: Arg.Is(command.Record.ExternalClosedOn.ToDateTimeOffset()),
                bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                validBet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
                commission: Arg.Is(0.0m),
                turnover: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
                playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
                additionalData: Arg.Is(command.Record.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
