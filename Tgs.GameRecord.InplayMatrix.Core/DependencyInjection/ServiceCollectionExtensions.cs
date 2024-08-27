using System.Reflection;
using Tgs.GameRecord.InplayMatrix.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
		.AddSingleton<IGameRecordDispatcher, GameRecordDispatcher>()
		.AddSingleton<IPullRecordByIMESportsDispatcher, PullRecordByIMESportsDispatcher>();
}
