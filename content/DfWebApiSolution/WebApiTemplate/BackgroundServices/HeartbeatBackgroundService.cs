using Microsoft.Extensions.Options;

namespace WebApiTemplate.BackgroundServices;

public sealed partial class HeartbeatBackgroundService : BackgroundService
{
    private readonly HeartbeatOptions _options;
    private readonly ILogger<HeartbeatBackgroundService> _logger;

    public HeartbeatBackgroundService(
        IOptions<HeartbeatOptions> options,
        ILogger<HeartbeatBackgroundService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogStarted(_logger);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.IntervalSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                LogHeartbeat(_logger);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            LogStopped(_logger);
        }
    }

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "Heartbeat background service started")]
    private static partial void LogStarted(ILogger logger);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Debug, Message = "Background-service heartbeat")]
    private static partial void LogHeartbeat(ILogger logger);

    [LoggerMessage(EventId = 2003, Level = LogLevel.Information, Message = "Heartbeat background service stopped")]
    private static partial void LogStopped(ILogger logger);
}
