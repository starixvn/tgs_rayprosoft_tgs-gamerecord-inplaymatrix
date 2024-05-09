using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest.ApplicationServices.ESports;

public class HandleRecordByIMESportsCommandServiceUnitTest
{
	[Fact]
	public async Task BetState_Is_Loss()
	{
		var faceLogger = NullLogger<HandleRecordByIMESportsCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordByIMESportsCommand(
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

		var service = new HandleRecordByIMESportsCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = "IM_ESP",
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn,
				ExternalClosedOn = command.Record.ExternalClosedOn,
				ExternalPayoffOn = command.Record.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 0.0M,
				Commission = 0.0M,
				Turnover = 5.0m,
				WinLoss = -5.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Win()
	{
		var faceLogger = NullLogger<HandleRecordByIMESportsCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordByIMESportsCommand(
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

		var service = new HandleRecordByIMESportsCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = "IM_ESP",
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn,
				ExternalClosedOn = command.Record.ExternalClosedOn,
				ExternalPayoffOn = command.Record.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 8.005m,
				Commission = 0.0M,
				Turnover = 5.0m,
				WinLoss = 3.005000m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running()
	{
		var faceLogger = NullLogger<HandleRecordByIMESportsCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordByIMESportsCommand(
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

		var service = new HandleRecordByIMESportsCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = "IM_ESP",
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn,
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 0.0m,
				Commission = 0.0M,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Void()
	{
		var faceLogger = NullLogger<HandleRecordByIMESportsCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new HandleRecordByIMESportsCommand(
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

		var service = new HandleRecordByIMESportsCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Handle.Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = "IM_ESP",
				PlayerCode = command.Record.PlayerCode,
				ExternalRoundId = command.Record.ExternalRoundId,
				ExternalBetId = command.Record.ExternalRoundId,
				ExternalGameId = command.Record.ExternalGameId,
				ExternalCreatedOn = command.Record.ExternalCreatedOn,
				ExternalClosedOn = command.Record.ExternalClosedOn,
				ExternalPayoffOn = command.Record.ExternalClosedOn,
				ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				BetState = Handle.Common.Protos.BetState.Void,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 5.0m,
				ValidBet = 5.0m,
				Settle = 0.0m,
				Commission = 0.0M,
				Turnover = 5.0m,
				WinLoss = 0.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Record.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
