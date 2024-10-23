using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UserServices.DTOs;
using UserServices.Models;

namespace UserServices.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserServicesContext _context;

        public UserRepository(UserServicesContext context)
        {
            _context = context;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(); 
        }

        public async Task<User> UpdateAsync(int id, UpdateDTO updateDTO)
        {
            var existingUser = await _context.Users.FindAsync(id);

            existingUser.Name = updateDTO.Name;
            existingUser.Email = updateDTO.Email;
            existingUser.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingUser;

        }

        
    }
}
