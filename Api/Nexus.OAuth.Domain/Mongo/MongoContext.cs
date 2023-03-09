using MongoDB.Bson;
using MongoDB.Driver;
using Nexus.OAuth.Dal.Models;
using Nexus.OAuth.Domain.Notifications.Models.Enums;

namespace Nexus.OAuth.Dal;
public class MongoContext : IDisposable
{
    public string ConnectionString { get; set; }
    private IMongoDatabase database;
    private const string NotificationsTable = "Notifications";
    private const string TwoFactorTable = "TwoFactor";
    private const string DataBase = "Nexus-OAuth";

    public MongoContext(string conn)
    {
        ConnectionString = conn;
        database = new MongoClient(ConnectionString)
            .GetDatabase(DataBase);
    }

    public async Task SendNotificationAsync(int userId, string title, string description, string channel, string category, string? activity)
    {
        var notification = new Notification(userId, title, description, channel, category)
        {
            Activity = activity
        };

        var colle = database.GetCollection<Notification>(NotificationsTable);

        await colle.InsertOneAsync(notification);
    }

    public async Task SendNotificationAsync(int userId, string title, string description, string channel, string category)
        => await SendNotificationAsync(userId, title, description, channel, category, null);

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

    /// <summary>
    /// Confirm in database two factor code.
    /// </summary>
    /// <returns></returns>
    public async Task ApplyTwoFactorAsync(TwoFactor code)
    {
        var colle = database.GetCollection<TwoFactor>(TwoFactorTable);

        await colle.InsertOneAsync(code);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="authId"></param>
    /// <returns></returns>
    public async Task<TwoFactor> GetTwoFactorAsync(int authId)
    {
        var colle = database.GetCollection<TwoFactor>(TwoFactorTable);

        return await colle.FindSync(tfa => tfa.AuthId == authId).FirstAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="authId"></param>
    /// <returns></returns>
    public async Task DeleteTwoFactorAsync(int authId)
    {
        var colle = database.GetCollection<TwoFactor>(TwoFactorTable);

        await colle.DeleteManyAsync(tfa => tfa.AuthId == authId);
    }

    /// <summary>
    /// Clean object
    /// </summary>
    public void Dispose()
    {
        ConnectionString = string.Empty;
        GC.SuppressFinalize(this);
    }
}