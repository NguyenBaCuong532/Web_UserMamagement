using Microsoft.EntityFrameworkCore;
using Web_App_UserManagement.Data;
using Web_App_UserManagement.Models;
using Web_App_UserManagement.Services;

namespace Web_App_UserManagement.Services
{
    public class DbUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbUserRepository> _logger;

        public DbUserRepository(ApplicationDbContext context, ILogger<DbUserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<User> GetAll()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public User? GetByCode(string code)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.Code == code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by code: {Code}", code);
                throw;
            }
        }

        public void Add(User user)
        {
            try
            {
                if (_context.Users.Any(u => u.Code == user.Code))
                {
                    throw new Exception("User with this Code already exists.");
                }

                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    throw new Exception("User with this Email already exists.");
                }

                _context.Users.Add(user);
                _context.SaveChanges();
                _logger.LogInformation("User added: {Code}", user.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Code}", user.Code);
                throw;
            }
        }

        public void Update(User user)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Code == user.Code);
                if (existingUser == null)
                {
                    throw new Exception($"User with code {user.Code} not found.");
                }

                // Check if email is being changed and if new email already exists
                if (existingUser.Email != user.Email && _context.Users.Any(u => u.Email == user.Email && u.Code != user.Code))
                {
                    throw new Exception("User with this Email already exists.");
                }

                existingUser.FullName = user.FullName;
                existingUser.DateOfBirth = user.DateOfBirth;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;

                _context.SaveChanges();
                _logger.LogInformation("User updated: {Code}", user.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Code}", user.Code);
                throw;
            }
        }

        public void Delete(string code)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Code == code);
                if (user == null)
                {
                    throw new Exception($"User with code {code} not found.");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();
                _logger.LogInformation("User deleted: {Code}", code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Code}", code);
                throw;
            }
        }
    }
}
