using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.InplayMatrix.Core.JobApplicationServices;

internal class JobInitCommandService(
    ILogger<JobInitCommandService> logger,
    IOptions<GameRecordInfoOptions> options,
    IHangfireLock hangfireLock,
    IGameCommandClient client) : ICommandService<JobInitCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

    private readonly RecurringJobOptions _recurringJobOptions = new()
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
    };

    public async ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await hangfireLock.LockingAsync(
                application: typeof(JobInitCommandService).Name,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                logger.LogWarning("LogOn:{logOn} Message:進行初始化動作，已經有其他服務進行", DateTime.Now.ToString("o"));
                return;
            }

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            foreach (var gameProviderCode in _gameRecordInfo.GameProviderCodes)
            {
                var gameProviderConfiguration = $"{gameProviderCode}Configuration";
                var settingConfiguration = await client.SettingGetConfigurationStringAsync(
                    key: gameProviderConfiguration,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(settingConfiguration))
                {
                    logger.LogError("LogOn:{logOn} Message:遊戲供應商({gameProviderCode})設定檔不存在({gameProviderConfiguration})。", DateTime.Now.ToString("o"), gameProviderCode, gameProviderConfiguration);
                    continue;
                }

                var settingConfigurationJson = JsonNode.Parse(settingConfiguration)
                    ?? throw new Exception($"This game product setting does not exist.({gameProviderConfiguration})");

                var gameProviderConfigs = await client.GameProviderConfigurationQueryAsync(
                    gameProviderCode: gameProviderCode,
                    cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false)
                    ?? Enumerable.Empty<GameProviderConfigInfo>();

                if (!gameProviderConfigs.Any())
                {
                    continue;
                }

                var gameProviderAgentMapper = new Dictionary<string, JobExecuteCommand>();

                foreach (var gameProviderConfig in gameProviderConfigs)
                {
                    if (string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData) || gameProviderConfig.Status == GameProviderConfigStatus.Inactive)
                    {
                        continue;
                    }

                    var configurationDataJson = JsonNode.Parse(gameProviderConfig.ConfigurationData)
                        ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfig.GameProviderConfigId})");

                    var reportUrl = new Uri(configurationDataJson["ReportUrl"]?.ToString()
                        ?? settingConfigurationJson["ReportUrl"]?.ToString()
                        ?? throw new Exception($"Required parameters(ReportUrl)"));
                    var merchantCode = configurationDataJson["MerchantCode"]?.ToString()
                        ?? throw new Exception($"Required parameters(MerchantCode)");
                    var merchantPrefix = configurationDataJson["MerchantPrefix"]?.ToString()
                        ?? throw new Exception($"Required parameters(MerchantPrefix)");

                    if (gameProviderAgentMapper.ContainsKey(merchantCode))
                    {
                        continue;
                    }

                    var jobExecuteCommand = new JobExecuteCommand(
                        GameProviderCode: gameProviderCode,
                        ReportUrl: reportUrl,
                        MerchantCode: merchantCode,
                        MerchantPrefix: merchantPrefix);

                    gameProviderAgentMapper.Add(merchantCode, jobExecuteCommand);

                    var recurringJobId = $"{gameProviderCode}|{gameProviderConfig.Name}|{merchantCode}|{gameProviderConfig.GameProviderConfigId}";

                    RecurringJob.AddOrUpdate<IHangfireUtensil>(
                        recurringJobId: recurringJobId,
                        methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
                        cronExpression: Cron.Minutely(),
                        options: _recurringJobOptions);

                    //初始化強制第一次處理
                    _ = BackgroundJob.Enqueue<IHangfireUtensil>(service => service.JobExecuteAsync(jobExecuteCommand));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} Message:遊戲廠商初始化排程發生例外", DateTime.Now.ToString("o"));
        }
        finally
        {
            _ = BackgroundJob.Schedule<IHangfireUtensil>(service => service.JobCheckAsync(), TimeSpan.FromMinutes(1));
        }
    }
}
