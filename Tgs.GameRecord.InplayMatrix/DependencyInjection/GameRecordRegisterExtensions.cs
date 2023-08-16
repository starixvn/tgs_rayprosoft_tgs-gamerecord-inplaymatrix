using Tgs.GameInterface.InplayMatrix.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class GameRecordRegisterExtensions
{
    public static async Task<WebApplication> UseGameRecordRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameInterfaceInplayMatrixClient = scope.ServiceProvider.GetRequiredService<IGameInterfaceInplayMatrixClient>();
            _ = await gameInterfaceInplayMatrixClient
                .CheckingServiceAsync(
                Environment.MachineName)
                .ConfigureAwait(false);
        }

        return app;
    }
}
