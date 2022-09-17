using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Models.Result;
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
        byte[] buffer;
        using var aux = new NotificationContext(ConnectionString);
        while (!sckt.CloseStatus.HasValue)
        {
            var status = new NotificationsStatusResult(await aux.GetNotificationsAsync(account.Id));

            buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(status));

            await sckt.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            Thread.Sleep(NotificationsConfigurations.AwaitCheck);
        }
    }

    private class NotificationsConfiguration
    {
        public int AwaitCheck { get; set; }
    }
}