using System;
using MongoDB.Driver;

namespace Tests
{
    public static class TestMongoDatabase
    {
        public static IMongoDatabase Create()
        {
            var mongoConnectionString = "mongodb+srv://admin:po1si2tron3@cluster0.bqnnl.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
            var mongoClient = new MongoClient(mongoConnectionString);
            return mongoClient.GetDatabase("game-tests");
        }
    }
}