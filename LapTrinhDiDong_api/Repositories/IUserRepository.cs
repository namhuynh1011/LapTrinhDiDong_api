using LapTrinhDiDong_api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LapTrinhDiDong_api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> AddUserAsync(User user);           
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
    }
}