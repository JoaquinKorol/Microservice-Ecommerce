using UserServices.DTOs;
using UserServices.Models;

namespace UserServices.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task<User> UpdateAsync(int id, UpdateDTO updateDTO);
        Task DeleteAsync(User user);
    }
}
