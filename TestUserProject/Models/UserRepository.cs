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
            try
            {
                var db = mongoClient.GetDatabase("myFirstDatabase");
                _users = db.GetCollection<User>(nameof(User));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error of connection to MongoDB:{e.Message}");
            }
        }

        public async Task<Guid> CreateAsync(User user)
        {
            var retv = Guid.Empty;
            try
            {
                await _users.InsertOneAsync(user);
                retv = user.UserId;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error of connection to MongoDB:{e.Message}");
            }
            return retv;
        }

        public List<User> GetAllUsers()
        {
            List<User> retv=new List<User>();
            try
            {
                retv = _users.Find(_ => true).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error of connection to MongoDB:{e.Message}");
            }
            return retv;
        }
    }
}
