using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Nexus.OAuth.Android.Assets.Api
{
    internal class NotificationsController : AuthenticatedController
    {
        public DateTime LastUpdate { get; private set; }
        public override string ControllerHost => $"{DefaultURL}/Notifications";
        public bool Connected => connected ? (!sckt.CloseStatus.HasValue) : connected;
        public event NewNotifications NewNotification;
        public event EventHandler<Exception> Error;
        private string socketUrl => $"{ControllerHost}/Connect".Replace("https", "wss");
        private const string TAG = "NotifyServiceConnection";
        private static bool started;
        private bool connected;
        ClientWebSocket sckt;
        Thread thRecive;
        public NotificationsController(Context context, Authentication authentication) : base(context, authentication)
        {
            sckt = new ClientWebSocket();
            thRecive = new Thread(StartReceive);
        }
        public void Connect()
        {
            if (started)
                throw new ArgumentException("Service called before is running");

            started = true;
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
                Log.Debug(TAG, $"Notify service connected success.");
#endif
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Notify service error {ex}");
                throw ex;
            }
        }
        private void StartReceive()
        {
            while (!sckt.CloseStatus.HasValue)
            {
                try
                {
                    WebSocketReceiveResult result;
                    var strMessage = string.Empty;
                    var buffer = new ArraySegment<byte>(new byte[4096]);

                    var ts = sckt.ReceiveAsync(buffer, CancellationToken.None);
                    ts.Wait();

                    result = ts.Result;

                    if (result.MessageType != WebSocketMessageType.Text)
                        break;
                    do
                    {
                        var messageBytes = buffer
                            .Take(result.Count)
                            .ToArray();

                        strMessage += Encoding.UTF8.GetString(messageBytes);
                    }
                    while (!result.EndOfMessage);

#if DEBUG
                    Log.Debug(TAG, $"Notify service response {strMessage}");
#endif

                    var status = JsonConvert.DeserializeObject<NotificationsStatusResult>(strMessage);

                    _ = Receive(status);

                    var upload = new NotificationsStatusUpload(status.Notifications.Select(notify => notify.Id).ToArray());

                    strMessage = JsonConvert.SerializeObject(upload);

                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(strMessage));

                    sckt.SendAsync(buffer, WebSocketMessageType.Text, false, CancellationToken.None)
                        .Wait();
                }
                catch (Exception ex)
                {
                    Log.Debug(TAG, $"Notify service error {ex}");
                    started = false;
                    Error.Invoke(this, ex);
                }
            }
#if DEBUG
            Log.Debug(TAG, $"Service Stopped");
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
            catch (Exception ex)
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