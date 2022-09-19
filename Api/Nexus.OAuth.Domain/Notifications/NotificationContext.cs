using MongoDB.Bson;
using MongoDB.Driver;
using Nexus.OAuth.Dal.Models;
using Nexus.OAuth.Domain.Notifications.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal;
public class NotificationContext : IDisposable
{

    public int MyProperty { get; set; }
    public string ConnectionString { get; set; }
    private IMongoDatabase database;
    private const string NotificationsTable = "Notifications";
    public NotificationContext(string conn)
    {
        ConnectionString = conn;
        database = new MongoClient(ConnectionString)
            .GetDatabase("OAuth-Notifications");
    }

    public async Task SendNotificationAsync(int userId, string title, string description, string channel)
    {
        var notification = new Notification(userId, title, description, channel);

        var colle = database.GetCollection<Notification>(NotificationsTable);

        await colle.InsertOneAsync(notification);
    }

    public async Task<Notification[]> GetNotificationsAsync(int userId)
    {
        var colle = database.GetCollection<Notification>(NotificationsTable);

        var response = await colle.FindAsync(fs => fs.UserId == userId && fs.Status == NotificationStatus.UnSended);

        var notifications = await response.ToListAsync();

        return notifications.ToArray();
    }

    public async Task NotifyReceiveds(string[] stringIds)
    {
        var ids = stringIds.Select(id => new ObjectId(id ?? string.Empty)).ToArray();

        try
        {
            var colle = database.GetCollection<Notification>(NotificationsTable);

            var arrayUpdate = Builders<Notification>.Update
                .Set(nameof(Notification.Status), NotificationStatus.Sended);

            await colle.UpdateManyAsync(not => ids.Contains(not.Id), arrayUpdate);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void Dispose()
    {
        ConnectionString = string.Empty;
        GC.SuppressFinalize(this);
    }
}