using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestUserProject.Models
{
    public interface IUserRepository
    {
        public Task<Guid> CreateAsync(User user);

        public IEnumerable<User> GetAllUsers();
    }
}
