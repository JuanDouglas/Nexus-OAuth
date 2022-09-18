using Android.Content;
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
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void StartReceive()
        {
            while (true)
            {
                WebSocketReceiveResult result;
                var message = new ArraySegment<byte>(new byte[4096]);
                do
                {
                    var ts = sckt.ReceiveAsync(message, CancellationToken.None);
                    ts.Wait();

                    result = ts.Result;

                    if (result.MessageType != WebSocketMessageType.Text)
                        break;

                    var messageBytes = message.Skip(message.Offset)
                        .Take(result.Count)
                        .ToArray();

                    string receivedMessage = Encoding.UTF8.GetString(messageBytes);

                    var status = Newtonsoft.Json.JsonConvert.DeserializeObject<NotificationsStatusResult>(receivedMessage);

                    Task receive = new Task(() => Receive(status));

                    receive.Start();
                }
                while (!result.EndOfMessage);
            }

        }

        private bool Receive(NotificationsStatusResult status)
        {
            LastUpdate = status.Date.ToLocalTime();

            try
            {
                if (status.Length > 0 && NewNotification != null)
                    NewNotification.Invoke(status.Date, status.Notifications);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public delegate void NewNotifications(DateTime time, Notification[] notifications);
    }
}