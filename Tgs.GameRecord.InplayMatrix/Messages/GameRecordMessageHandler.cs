using System.Text;
using System.Text.Json;
using Tgs.GameRecord.InplayMatrix.Common;
using Tgs.GameRecord.InplayMatrix.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.InplayMatrix.Messages;

internal class GameRecordMessageHandler(
    ILogger<GameRecordMessageHandler> logger,
    IGameRecordDispatcher dispatcher) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var message = JsonSerializer.Deserialize<InplayMatrixGameRecordMessage>(json)
        ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{message}", DateTime.Now.ToString("o"), message);

        var command = new GameRecordCommand(
            GameProviderCode: message.GameProviderCode.ToUpper(),
            ReportUrl: message.ReportUrl,
            MerchantCode: message.MerchantCode,
            MerchantPrefix: message.MerchantPrefix,
            StartTime: message.StartTime,
            EndTime: message.EndTime);

        return dispatcher.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
