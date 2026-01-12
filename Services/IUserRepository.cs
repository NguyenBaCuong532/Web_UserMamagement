using Web_App_UserManagement.Models;

namespace Web_App_UserManagement.Services
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetByCode(string code);
        void Add(User user);
        void Update(User user);
        void Delete(string code);
    }
}
