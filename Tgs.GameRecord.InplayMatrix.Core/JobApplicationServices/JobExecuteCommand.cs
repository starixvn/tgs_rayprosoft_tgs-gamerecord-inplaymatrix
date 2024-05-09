namespace Tgs.GameRecord.InplayMatrix.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	Uri ReportUrl,
	string MerchantCode,
	string MerchantPrefix);
