using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest.ApplicationServices.ESports;

public class PullRecordByIMESportsCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<PullRecordByIMESportsCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceInplayMatrixClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordByIMESportsCommand>>();

		var command = new PullRecordByIMESportsCommand(
			ReportUrl: new("http://localhost/"),
			MerchantCode: "merchantCode",
			MerchantPrefix: "merchantPrefix",
			StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndTime: DateTimeOffset.UtcNow);

		var service = new PullRecordByIMESportsCommandService(
			fakeLogger,
			fakeClient,
			fakeService);

		await service.ExecuteAsync(command: command);

		_ = await fakeClient
			.Received(1)
			.GameRecordByIMESportsAsync(
			reportUrl: Arg.Is(command.ReportUrl),
			merchantCode: Arg.Is(command.MerchantCode),
			merchantPrefix: Arg.Is(command.MerchantPrefix),
			startDate: Arg.Is(command.StartTime),
			endDate: Arg.Is(command.EndTime),
			page: Arg.Any<int>(),
			cancellationToken: Arg.Any<CancellationToken>())
			.ToArrayAsync(
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
