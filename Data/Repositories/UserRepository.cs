using Domain.Models;
using Mapster;
using Repository.Data;
using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users.Adapt<IEnumerable<User>>();
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user.Adapt<User>();
        }

        public async Task<User> CreateUser(User user)
        {
            var userEntity = user.Adapt<UserEntity>();
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            return userEntity.Adapt<User>();
        }

        public async Task<User> UpdateUser(User user)
        {
            var existingUser = await _context.Users.FirstAsync(u => u.Id == user.Id);
            user.Adapt(existingUser);
            await _context.SaveChangesAsync();
            return existingUser.Adapt<User>();
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
