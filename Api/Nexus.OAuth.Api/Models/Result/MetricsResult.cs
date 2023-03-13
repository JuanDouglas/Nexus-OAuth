namespace Nexus.OAuth.Api.Models.Result;
public class MetricsResult
{
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public int Requests { get; set; }
    public int RequestsFailure { get; set; }
    public double RequestPerSecond { get; set; }
}