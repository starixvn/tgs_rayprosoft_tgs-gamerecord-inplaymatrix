using Tgs.GameRecord.InplayMatrix.Core;

using Tgs.GameRecord.InplayMatrix.Core.JobApplicationServices;

using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Repositories;

internal class HangfireUtensilRepository(
	ICommandService<JobCheckCommand> jobCheckService,
	ICommandService<JobExecuteCommand> jobExecuteService,
	ICommandService<JobInitCommand> jobInitservice) : IHangfireUtensilRepository
{
	public Task JobCheckAsync()
		=> jobCheckService.ExecuteAsync(new()).AsTask();

	public Task JobExecuteAsync(JobExecuteCommand source)
		=> jobExecuteService.ExecuteAsync(source).AsTask();

	public Task JobInitAsync()
		=> jobInitservice.ExecuteAsync(new()).AsTask();
}
