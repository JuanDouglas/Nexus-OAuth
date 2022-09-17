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
    public NotificationContext(string conn)
    {
        ConnectionString = conn;
        database = new MongoClient(ConnectionString)
            .GetDatabase("OAuth-Notifications");
    }

    public async Task SendNotificationAsync(int userId, string title, string description, string channel)
    {
        var notification = new Notification(userId, title, description, channel);

        var colle = database.GetCollection<Notification>("Notifications");

        await colle.InsertOneAsync(notification);
    }

    public async Task<Notification[]> GetNotificationsAsync(int userId)
    {
        var colle = database.GetCollection<Notification>("Notifications");

        var response = await colle.FindAsync(fs => fs.UserId == userId && fs.Status == NotificationStatus.UnSended);

        var notifications = await response.ToListAsync();

        var ids = notifications
            .Select(not => not.Id)
            .ToArray();

        var arrayUpdate = Builders<Notification>.Update
            .Set(nameof(Notification.Status), NotificationStatus.Sended);

        colle.UpdateMany(not => ids.Contains(not.Id), arrayUpdate);

        return notifications.ToArray();
    }

    public void Dispose()
    {
        ConnectionString = string.Empty;
        GC.SuppressFinalize(this);
    }
}