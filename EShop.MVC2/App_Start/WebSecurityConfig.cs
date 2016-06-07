using WebMatrix.WebData;
using EShop.MVC2.Models;
using System.Web.Security;

namespace EShop.MVC2.App_Start
{
    public class WebSecurityConfig
    {
        public static void Config()
        {
            if(!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("eshop.db", "UserEntity", "Id", "Login", autoCreateTables: true);
                System.Data.Entity.Database.SetInitializer <UserEntityContext>(null);
                
            }

            if(!Roles.RoleExists("Admins")) { Roles.CreateRole("Admins"); }
            if (!Roles.RoleExists("Users")) { Roles.CreateRole("Users"); }
            if(!WebSecurity.UserExists("Admin"))
            {
                WebSecurity.CreateUserAndAccount("Admin", "secret",
                    new
                    {
                        Email = "admin@eshop.by",
                        FullName = "Admin Vasya",
                        IsActive = true,
                        IsApproved = true
                    });
            }
            if (!Roles.IsUserInRole("admin", "Admins"))
                Roles.AddUserToRole("admin", "Admins");
        }
    }
}