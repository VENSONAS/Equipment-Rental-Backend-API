using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<User> CreateUser(User user)
        {
            var createdUser = await _userRepository.CreateUser(user);

            createdUser.SetCreatedTime();

            _logger.LogInformation($"User created with ID {user.Id}");
            return createdUser;
        }

        public async Task<User> UpdateUser(int id, User user)
        {
            var userToUpdate = await _userRepository.GetUserById(id);

            userToUpdate.ChangeName(user.Name);
            userToUpdate.ChangeEmail(user.Email);

            return await _userRepository.UpdateUser(userToUpdate);
        }

        public async Task DeleteUser(int id)
        {
            await _userRepository.DeleteUser(id);
        }
    }
}
