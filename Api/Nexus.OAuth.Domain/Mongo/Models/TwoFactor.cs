using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nexus.OAuth.Dal.Models.Enums;

namespace Nexus.OAuth.Dal.Models;
public class TwoFactor
{
    [BsonId]
    public ObjectId Id { get; private set; }
    public DateTime Send { get; set; }
    public TwoFactorType Type { get; set; }
    public string Code { get; set; }
    public int AuthId { get; set; }
}