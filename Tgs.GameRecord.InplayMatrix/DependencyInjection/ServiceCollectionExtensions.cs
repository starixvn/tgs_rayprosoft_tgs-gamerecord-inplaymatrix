using Tgs.GameCommand.Client;
using Tgs.GameInterface.InplayMatrix.Client;
using Tgs.GameRecord.InplayMatrix;
using Tgs.GameRecord.InplayMatrix.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecord(
		this IServiceCollection services,
		Action<RedisOptions, IServiceProvider> redisDBconfigure,
		Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceInplayMatrixClientOptions, IServiceProvider> gameInterfaceInplayMatrixClientOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisDBconfigure)
		.Services
		.AddOptions<GameRecordInfoOptions>()
		.Configure(gameRecordInfoOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddGameRecordCore(
			gameCommandClientOptions: gameCommandClientOptions,
			gameInterfaceInplayMatrixClientOptions: gameInterfaceInplayMatrixClientOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
