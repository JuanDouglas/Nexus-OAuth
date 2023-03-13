using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexus.OAuth.Api;
using Nexus.OAuth.Api.Controllers.Base;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

[AllowAnonymous]
[Route("api/[controller]")]
public class MetricsController : ApiController
{
    private readonly ILogger<MetricsController> _logger;
    private readonly IPerfomaceService perfomaceService;
    public MetricsController(IHttpContextAccessor httpContextAccessor, ILogger<MetricsController> logger, IConfiguration config)
        : base(config)
    {
        _logger = logger;
        perfomaceService = (IPerfomaceService)httpContextAccessor.HttpContext.Items[nameof(PerfomaceService)];
    }

    [HttpGet]
    [Route("Requests")]
    public async Task Requests()
    {
        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024 * 4];
        var cancellationToken = HttpContext.RequestAborted;
        try
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                MetricsResult _serverMetrics = new()
                {
                    CpuUsage = perfomaceService.CpuUsage,
                    MemoryUsage = perfomaceService.MemoryUsage,
                    Requests = perfomaceService.RequestCount,
                    RequestsFailure = perfomaceService.FailedRequestCount,
                    RequestPerSecond = perfomaceService.RequestsPerSecond
                };

                buffer = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(_serverMetrics));
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, cancellationToken);

                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending metrics over WebSocket.");
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed.", cancellationToken);
        }
    }
}