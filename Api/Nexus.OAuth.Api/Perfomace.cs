using OpenHardwareMonitor.Hardware;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Nexus.OAuth.Api;
public interface IPerfomaceService
{
    public int RequestCount { get; }
    public int FailedRequestCount { get; }
    public double RequestsPerSecond { get; }
    public double CpuUsage { get; }
    public double MemoryUsage { get; }
    public double AverageResponseTime { get; }
}
internal class PerfomaceService : IPerfomaceService
{
    private readonly MetricsMiddleware _middleware;

    public PerfomaceService(MetricsMiddleware middleware)
    {
        _middleware = middleware;
    }

    public int RequestCount
        => _middleware.RequestCount;
    public int FailedRequestCount
        => _middleware.FailedRequestCount;
    public double RequestsPerSecond
        => _middleware.RequestsPerSecond;
    public double AverageResponseTime
        => _middleware.AverageResponseTime;
    public double CpuUsage
        => _middleware.CpuUsage;

    public double MemoryUsage
        => _middleware.MemoryUsage;
}
internal class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private int _requestCount = 0;
    private int _successfulRequestCount = 0;
    private int _failedRequestCount = 0;
    private int _requestsPerSecond = 0;
    private float _totalResponseTime;
    private Dictionary<HttpMethodType, float> _httpMethodCounts;
    private Timer _timer;
    private Computer computer;
    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
        computer = new Computer
        {
            CPUEnabled = true,
            RAMEnabled = true
        };
        computer.Open();

        // Inicializa o objeto Timer para atualizar as requisições por segundo a cada segundo.
        _timer = new Timer(1000);
        _timer.Elapsed += (sender, args) =>
        {
            _requestsPerSecond = _requestCount;
            _requestCount = 0;
        };
        _timer.Start();
        _httpMethodCounts = new Dictionary<HttpMethodType, float>
            {
                { HttpMethodType.Get, 0 },
                { HttpMethodType.Post, 0 },
                { HttpMethodType.Put, 0 },
                { HttpMethodType.Options, 0 },
                { HttpMethodType.Patch, 0 },
                { HttpMethodType.Connect, 0 },
                { HttpMethodType.Head, 0 },
                { HttpMethodType.Trace, 0 },
                { HttpMethodType.Delete, 0 }
            };
    }

    public async Task Invoke(HttpContext context)
    {
        _requestCount++;

        context.Items.Add(nameof(PerfomaceService), new PerfomaceService(this));
        var httpMethodString = context.Request.Method;
        Enum.TryParse(httpMethodString, true, out HttpMethodType httpMethodEnum);

        _httpMethodCounts[httpMethodEnum]++;

        var watch = Stopwatch.StartNew();
        try
        {
            await _next(context);
            _successfulRequestCount++;
        }
        catch
        {
            _failedRequestCount++;
            throw;
        }
        finally
        {
            _timer.Stop();
        }
    }


    public Dictionary<HttpMethodType, float> MethodsCount
        => _httpMethodCounts;

    public int RequestCount
        => _successfulRequestCount + _failedRequestCount;

    public int FailedRequestCount
        => _failedRequestCount;

    public double RequestsPerSecond
        => _requestsPerSecond;

    public double AverageResponseTime
        => _totalResponseTime / RequestCount;

    public double CpuUsage
    {
        get
        {
            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.CPU);
            cpu.Update();
            return cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value ?? 0;
        }
    }
    public double MemoryUsage
    {
        get
        {
            var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.RAM);
            cpu.Update();
            return cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load)?.Value ?? 0;
        }
    }

    public enum HttpMethodType
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Options,
        Trace,
        Connect,
        Patch
    }
}