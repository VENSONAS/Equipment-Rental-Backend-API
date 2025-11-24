using Domain.Models;
using Microsoft.Extensions.Logging;
using Repository;
using System;
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
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers()
                ?? throw new InvalidOperationException("Failed to retrieve users.");

            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            var user = await _userRepository.GetUserById(id)
                ?? throw new InvalidOperationException($"User with ID {id} not found.");

            return user;
        }

        public async Task<User> CreateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var createdUser = await _userRepository.CreateUser(user)
                ?? throw new InvalidOperationException("Failed to create user.");

            createdUser.SetCreatedTime();

            _logger.LogInformation($"User created with ID {createdUser.Id}");

            return createdUser;
        }

        public async Task<User> UpdateUser(int id, User user)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userToUpdate = await _userRepository.GetUserById(id)
                ?? throw new InvalidOperationException($"User with ID {id} not found.");

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new InvalidOperationException("User name cannot be empty.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new InvalidOperationException("User email cannot be empty.");

            userToUpdate.ChangeName(user.Name);
            userToUpdate.ChangeEmail(user.Email);

            var updatedUser = await _userRepository.UpdateUser(userToUpdate)
                ?? throw new InvalidOperationException("Failed to update user.");

            return updatedUser;
        }

        public async Task DeleteUser(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            await _userRepository.DeleteUser(id);
        }
    }
}
