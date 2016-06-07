using System.Collections;
using System.Collections.Generic;
using System.Linq;

// См. 11.2.1 шаг 10
namespace EShop.MVC2.Models
{
    public interface IUserManager
    {
        UserModel GetUser(string userName);
        UserModel GetUser(int userId);
        void SetUser(UserModel user);
        IEnumerable<UserModel> UserList();
        bool LoginUser(string userName, string password);
        IQueryable<UserModel> Users { get; }
        void DeleteAccount(UserModel user);
    }
}