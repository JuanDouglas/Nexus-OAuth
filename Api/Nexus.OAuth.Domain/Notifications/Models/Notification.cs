using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexus.OAuth.Domain.Notifications.Models.Enums;

namespace Nexus.OAuth.Dal.Models
{
    public class Notification
    {
        [BsonId]
        public ObjectId Id { get; private set; }
        public int UserId { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime Date { get; set; }

        public string? Activity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Channel { get; set; }

        public Notification()
        {
        }

        public Notification(int userId, string title, string description, string channel)
        {
            UserId = userId;
            Title = title;
            Description = description;
            Channel = channel;
            Date = DateTime.UtcNow;
            Status = NotificationStatus.UnSended;
        }
    }
}
