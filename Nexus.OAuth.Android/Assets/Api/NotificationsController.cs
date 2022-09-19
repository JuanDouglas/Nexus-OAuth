using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Android.Content.ClipData;

namespace Nexus.OAuth.Android.Assets.Api
{
    internal class NotificationsController : AuthenticatedController
    {
        public DateTime LastUpdate { get; private set; }
        public override string ControllerHost => $"{DefaultURL}/Notifications";

        public event NewNotifications NewNotification;
        private string socketUrl => $"{ControllerHost}/Connect".Replace("https", "wss");
        ClientWebSocket sckt;
        Thread thRecive;
        public NotificationsController(Context context, Authentication authentication) : base(context, authentication)
        {
            sckt = new ClientWebSocket();
            thRecive = new Thread(StartReceive);
        }
        public void Connect()
        {
            foreach (var item in BaseRequest.Headers)
            {
                sckt.Options.SetRequestHeader(item.Key, string.Join(" ", item.Value));
            }

            try
            {
                sckt.ConnectAsync(new Uri(socketUrl), CancellationToken.None)
                    .Wait();

                thRecive.Start();

#if DEBUG
                Log.Debug("NotifyService", $"Notify service connected success.");
#endif
            }
            catch (Exception ex)
            {
                Log.Debug($"NotifyService", $"Notify service error {ex}");
                throw ex;
            }
        }
        private void StartReceive()
        {
            Log.Debug($"NotifyService", $"Socket close status {sckt.CloseStatus}");
            while (!sckt.CloseStatus.HasValue)
            {
                try
                {
                    WebSocketReceiveResult result;
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        var ts = sckt.ReceiveAsync(buffer, CancellationToken.None);
                        ts.Wait();

                        result = ts.Result;

                        if (result.MessageType != WebSocketMessageType.Text)
                            break;

                        var messageBytes = buffer.Skip(buffer.Offset)
                            .Take(result.Count)
                            .ToArray();

                        string strMessage = Encoding.UTF8.GetString(messageBytes);

                        Log.Debug($"NotifyService", $"Notify service response {strMessage}");

                        var status = JsonConvert.DeserializeObject<NotificationsStatusResult>(strMessage);

                        _ = Receive(status);

                        var upload = new NotificationsStatusUpload(status.Notifications.Select(notify => notify.Id).ToArray());

                        strMessage = JsonConvert.SerializeObject(upload);

                        buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage));

                        sckt.SendAsync(buffer, WebSocketMessageType.Text, false, CancellationToken.None)
                            .Wait();
                    }
                    while (!result.EndOfMessage);
                }
                catch (Exception ex)
                {
                    Log.Debug($"NotifyService", $"Notify service error {ex}");
                    throw;
                }
            }
#if DEBUG
            Log.Debug("NotifyService", $"Service Stopped");
#endif
        }

        private bool Receive(NotificationsStatusResult status)
        {
            LastUpdate = status.Date.ToLocalTime();

            try
            {
                if (status.Length > 0 && NewNotification != null)
                    NewNotification.Invoke(status.Date, status.Notifications);
#if DEBUG
                else
                    Log.Debug($"NotifyService", $" No notifications.");
#endif
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public delegate void NewNotifications(DateTime time, Notification[] notifications);

        private class NotificationsStatusUpload
        {
            public string[] Receiveds { get; set; }

            public NotificationsStatusUpload(string[] receiveds)
            {
                Receiveds = receiveds ?? throw new ArgumentNullException(nameof(receiveds));
            }
        }
    }
}