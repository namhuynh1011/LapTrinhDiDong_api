using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LapTrinhDiDong_api.Data;
using LapTrinhDiDong_api.Models;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhDiDong_api.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _context.Users.Where(u => u.Role != 0).ToListAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetUserByIdAsync(Guid id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<User> AddUserAsync(User user)
    {
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
      return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
      return user;
    }

    public async Task DeleteUserAsync(Guid id)
    {
      var user = await GetUserByIdAsync(id);
      if (user != null)
      {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
      }
    }
  }
}