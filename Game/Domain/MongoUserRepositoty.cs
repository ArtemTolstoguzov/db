using System;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);
            userCollection.Indexes.CreateOne(
                new CreateIndexModel<UserEntity>(
                    Builders<UserEntity>.IndexKeys.Ascending(user => user.Login),
                    new CreateIndexOptions { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            return userCollection.Find(user => user.Id == id).FirstOrDefault();
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            var user = userCollection.FindSync(u => u.Login == login).FirstOrDefault();
            if (user != null)
            {
                return user;
            }

            var newUser = new UserEntity(Guid.NewGuid())
            {
                Login = login
            };
            return Insert(newUser);
        }

        public void Update(UserEntity user)
        {
            userCollection.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var count = userCollection.CountDocuments(u => true);
            var users = userCollection
                .Find(user => true)
                .SortBy(user => user.Login)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(users, count, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}