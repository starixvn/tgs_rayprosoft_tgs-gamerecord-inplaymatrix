using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.InplayMatrix.Protos;
using Tgs.GameRecord.InplayMatrix.Core;

namespace Tgs.GameRecord.FaChai.Core.UnitTest;

public class GameRecordByESportsUtensilUnitTest
{
    [Fact]
    public async Task Cancelled_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordByIMESportsInfo
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
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetESportsGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "IM_ESP";
        _ = fakeUtensil
            .GetESportsGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.ExternalGameId.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordByESportsUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .GameRecordConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.True(actual.Void);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Settlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordByIMESportsInfo
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
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetESportsGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "IM_ESP";
        _ = fakeUtensil
            .GetESportsGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.ExternalGameId.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordByESportsUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .GameRecordConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Unsettlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameRecordByIMESportsInfo
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
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetESportsGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "IM_ESP";
        _ = fakeUtensil
            .GetESportsGameProviderCode()
            .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: source.PlayerCode,
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(source.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.ExternalGameId.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordByESportsUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .GameRecordConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.ExternalGameId.ToString(), actual.ExternalGameId);
        Assert.Equal(source.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.False(actual.Closed);
        Assert.False(actual.Void);
        Assert.Equal(source.ExternalCreatedOn.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.ExternalClosedOn.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.WinLoss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Settle.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }
}
