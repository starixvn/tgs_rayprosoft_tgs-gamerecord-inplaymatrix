using NSubstitute;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest;

public class GameRecordDispatcherUnitTest
{
	[Fact]
	public async Task IM_ESP()
	{
		var fakePullRecordByIMESportsDispatcher = Substitute.For<IPullRecordByIMESportsDispatcher>();

		var command = new GameRecordCommand(
			GameProviderCode: "IM_ESP",
			ReportUrl: new("http://localhost/"),
			MerchantCode: "merchantCode",
			MerchantPrefix: "merchantPrefix",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndTime: DateTimeOffset.UtcNow);

		var sut = new GameRecordDispatcher(
			fakePullRecordByIMESportsDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakePullRecordByIMESportsDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is<PullRecordByIMESportsCommand>(
				compare => compare.ReportUrl == command.ReportUrl &&
				compare.MerchantCode == command.MerchantCode &&
				compare.MerchantPrefix == command.MerchantPrefix &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Exception_NotSupportedException()
	{
		var fakePullRecordByIMESportsDispatcher = Substitute.For<IPullRecordByIMESportsDispatcher>();

		var command = new GameRecordCommand(
			GameProviderCode: "IM",
			ReportUrl: new("http://localhost/"),
			MerchantCode: "merchantCode",
			MerchantPrefix: "merchantPrefix",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndTime: DateTimeOffset.UtcNow);

		var sut = new GameRecordDispatcher(
			fakePullRecordByIMESportsDispatcher);

		_ = await Assert.ThrowsAsync<NotSupportedException>(() => sut.ExecuteAsync(command: command).AsTask());

		_ = fakePullRecordByIMESportsDispatcher
			.Received(0)
			.PushCommandAsync(
			command: Arg.Is<PullRecordByIMESportsCommand>(
				compare => compare.ReportUrl == command.ReportUrl &&
				compare.MerchantCode == command.MerchantCode &&
				compare.MerchantPrefix == command.MerchantPrefix &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
