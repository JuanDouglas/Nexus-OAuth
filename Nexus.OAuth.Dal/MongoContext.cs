using MongoDB.Driver;

namespace Nexus.OAuth.Dal
{
    public class MongoDataContext
    {
        public MongoDataContext(string connectionString)
        {
            MongoClient client = new();
        }
    }
}
