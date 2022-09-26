using Newtonsoft.Json;
using Nexus.OAuth.Api.Controllers.Base;
using System.Net.WebSockets;
using System.Text;

namespace Nexus.OAuth.Api.Controllers;

[Route("api/Notifications")]
[RequireAuthentication(RequireAccountValidation = true, RequiresToBeOwner = true, MinAuthenticationLevel = (int)Scope.User)]
public sealed class NotificationsController : ApiController
{
    public string ConnectionString { get; set; }
    private NotificationsConfiguration NotificationsConfigurations { get; set; }
    public NotificationsController(IConfiguration configuration) : base(configuration)
    {
        NotificationsConfigurations = configuration.GetSection("Notifications").Get<NotificationsConfiguration>();
        ConnectionString = configuration.GetConnectionString("NotificationsServer");
    }

    [HttpGet]
    [Route("Connect")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task ConnectAsync()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            if (ClientAccount == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (ClientAccount.ConfirmationStatus < ConfirmationStatus.EmailSucess)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            await AwaitNotificationAsync(socket, ClientAccount);
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    private async Task AwaitNotificationAsync(WebSocket sckt, Account account)
    {
        byte[] buffer = Array.Empty<byte>();
        string text;
        using var aux = new NotificationContext(ConnectionString);
        while (!sckt.CloseStatus.HasValue)
        {
            dynamic status = new NotificationsStatusResult(await aux.GetNotificationsAsync(account.Id));

            text = JsonConvert.SerializeObject(status);

            buffer = Encoding.UTF8.GetBytes(text);

            await sckt.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            buffer = new byte[buffer.Length];

            await sckt.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            text = Encoding.UTF8.GetString(buffer);

            status = JsonConvert.DeserializeObject<NotificationsStatusUpload>(text);

            await aux.NotifyReceiveds(status?.Receiveds ?? Array.Empty<string>());

            Thread.Sleep(NotificationsConfigurations.AwaitCheck);
        }
    }
    private class NotificationsConfiguration
    {
        public int AwaitCheck { get; set; }
    }
}