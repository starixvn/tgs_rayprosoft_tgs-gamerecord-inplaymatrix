using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest.ApplicationServices.ESports;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceInplayMatrixClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordCommand(
            ReportUrl: new("http://localhost/"),
            MerchantCode: "merchantCode",
            MerchantPrefix: "merchantPrefix",
            StartDate: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndDate: DateTimeOffset.UtcNow);

        var service = new PullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameRecordByIMESportsAsync(
                reportUrl: Arg.Is(command.ReportUrl),
                merchantCode: Arg.Is(command.MerchantCode),
                merchantPrefix: Arg.Is(command.MerchantPrefix),
                startDate: Arg.Is(command.StartDate),
                endDate: Arg.Is(command.EndDate),
                page: Arg.Any<int>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .ToArrayAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
