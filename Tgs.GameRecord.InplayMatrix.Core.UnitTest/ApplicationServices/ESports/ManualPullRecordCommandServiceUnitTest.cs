using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameRecord.InplayMatrix.Core.ApplicationServices.ESports;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.UnitTest.ApplicationServices.ESports;

public class ManualPullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<ManualPullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceInplayMatrixClient>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordByESportsUtensil>();
        var fakeService = Substitute.For<ICommandService<ManualHandleRecordCommand>>();

        var command = new ManualPullRecordCommand(
            ReportUrl: new("http://localhost/"),
            MerchantCode: "merchantCode",
            MerchantPrefix: "merchantPrefix",
            StartDate: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndDate: DateTimeOffset.UtcNow);

        var service = new ManualPullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameRecordAsync(
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
