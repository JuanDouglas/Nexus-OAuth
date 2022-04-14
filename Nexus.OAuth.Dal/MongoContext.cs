using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
