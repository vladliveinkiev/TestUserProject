using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestUserProject.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository (IMongoClient mongoClient)
        {
            var db = mongoClient.GetDatabase("myFirstDatabase");
            _users = db.GetCollection<User>(nameof(User));
        }

        public async Task<Guid> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user.UserId;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users.Find(_ => true).ToEnumerable();
        }
    }
}
