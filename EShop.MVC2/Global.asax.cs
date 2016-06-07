using EShop.Entity;
using EShop.MVC2.App_Start;
using EShop.MVC2.Environment;
using EShop.MVC2.Models;
using EShop.ServiceLayer;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EShop.MVC2
{
    //см 12.2 шаг 2
    public class MvcApplication : HttpApplication
    {
        private static IUserManager userManager;

        public static IUserManager UserManager
        {
            get
            {
                if (userManager == null)
                {
                    userManager = new MembershipUserStorage();
                }
                return userManager;
            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebSecurityConfig.Config();
        }
    }
}
