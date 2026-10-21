namespace WebApiTemplate.BackgroundServices;

public sealed class HeartbeatOptions
{
    public const string SectionName = "BackgroundServices:Heartbeat";

    public bool Enabled { get; init; } = true;

    public int IntervalSeconds { get; init; } = 300;
}
