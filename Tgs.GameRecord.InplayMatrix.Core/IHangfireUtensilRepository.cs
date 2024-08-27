using Tgs.GameRecord.InplayMatrix.Core.JobApplicationServices;

namespace Tgs.GameRecord.InplayMatrix.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
