using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task DeleteUser(int id);
    }
}
