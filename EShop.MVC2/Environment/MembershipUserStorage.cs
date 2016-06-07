using EShop.MVC2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

//См. 12.2
namespace EShop.MVC2.Environment
{
    public class MembershipUserStorage : IUserManager
    {
        public UserModel GetUser (string userName)
        {
            using (var db = new UserEntityContext())
            {
                return db.UserEntities.FirstOrDefault(u => u.Login.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }
        }
        public UserModel GetUser(int userId)
        {
            using (var db = new UserEntityContext())
            {
                return db.UserEntities.FirstOrDefault(u => u.UserId == userId);
            }
        }

        public void SetUser(UserModel user)
        {
            //WebSecurity.CreateUserAndAccount("Test12345", "password12345", new { Email = user.Email, FullName = user.FullName, IsActive = user.IsActive });
            try
            {
                if (!WebSecurity.UserExists(user.Login))
                {
                    WebSecurity.CreateUserAndAccount(user.Login, user.Password, new
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        IsActive = user.IsActive,
                        IsApproved = user.IsApproved
                    });
                    Roles.AddUserToRole(user.Login, "Users");
                }
                else
                {
                    using (var db = new UserEntityContext())
                    {
                        var userEntity = db.UserEntities.FirstOrDefault(u => u.Login.Equals(user.Login));
                        if (userEntity == null) return;
                        userEntity.FullName = user.FullName;
                        userEntity.IsActive = user.IsActive;
                        userEntity.Email = user.Email;
                        userEntity.IsApproved = user.IsApproved;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { }
        }

        public IEnumerable<UserModel> UserList()
        {
            using (var db = new UserEntityContext())
            {
                return db.UserEntities.ToArray();
            }
        }

        public bool LoginUser(string userName, string password)
        {
            var result = WebSecurity.Login(userName, password, false);
            return result;
        }
        public IQueryable<UserModel> Users
        {
            get
            {
                using (var db = new UserEntityContext())
                {
                    return db.UserEntities;
                }
            }
        }

        public void DeleteAccount (UserModel user)
        {
            //вот такая процедура удаления аккаунта встретилась на stackoverflow
            string[] allRoles = Roles.GetRolesForUser(user.Login);
            Roles.RemoveUserFromRoles(user.Login, allRoles);
            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.Login); // deletes record from webpages_Membership table
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.Login, true);// deletes record from UserProfile table

        }
    }
}