using EShop.Entity;
using EShop.MVC2.Models;
using EShop.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace EShop.MVC2.Controllers
{
    public class AccountController : Controller
    {
        //private static string email;
        private IDataService dataService;
        Random rnd = new Random();

        public AccountController (IDataService service)
        {
            dataService = service;
        }

        // GET: Account
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginView user, string returnUrl)
        {
            if(ModelState.IsValid && MvcApplication.UserManager.LoginUser(user.Login, user.Password))//проверка ввода логина и пароля и существования такого мембера
            {
                //if(user)
                var loginUser = MvcApplication.UserManager.GetUser(user.Login);
                if(!loginUser.IsActive)//проверка забанен ли такой аккаунт
                {
                    ModelState.AddModelError("", "Ваша учетная запись заблокирована");
                    return View(user);
                }
                if (!loginUser.IsApproved)
                {
                    ModelState.AddModelError("", "Ваша учетная запись еще не активирована. Проверьте свою электронную почту.");
                    return View(user);
                }
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "Логин или пароль не верны");
            
            return View(user);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {                
                return Redirect(returnUrl);
            }
            else
            {
                //если вдруг попадем сюда, то надо определиться куда перенаправлять запрос далее
                return RedirectToAction("", "");
            }
        }

        //Упражнение 11.2.1 шаг 16
        [HttpGet]
        public ActionResult Users()
        {
            var userId = 0;
            UserModel user = null;
            if (RouteData.Values.ContainsKey("id"))
            {
                if (int.TryParse(RouteData.Values["id"].ToString(), out userId))
                {
                    var userModel = MvcApplication.UserManager.GetUser(userId);
                    user = new UserModel
                    {
                        FullName = userModel.FullName,
                        Password = userModel.Password,
                        Email = userModel.Email,
                        Login = userModel.Login,
                        IsActive = userModel.IsActive
                    };
                }
            }
            ViewBag.UserList = MvcApplication.UserManager.UserList();
            return View("Users", user);
        }

        //Упражнение 11.2.2 шаг 7
        [HttpPost]
        public ActionResult Users(UserModel user)
        {
            if (this.ModelState.IsValid)
            {
                var oldUser = MvcApplication.UserManager.GetUser(user.Login);
                if(oldUser == null)//проверка есть ли в базе юзер с таким логином
                {
                    MvcApplication.UserManager.SetUser(new UserModel
                    {
                        //UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        Login = user.Login,
                        Password = user.Password,
                        IsActive = true,
                        IsApproved = false
                    });
                    //получаем Id свежезарегенного юзера
                    var newUser = MvcApplication.UserManager.GetUser(user.Login);
                    //и отправляем его вместе с адресом эл. почты в метод public ActionResult NewUser(string email, int id)
                    //для подтверждения регистрации
                    return new RedirectResult(Url.Action("NewUser", "Account", new { id = newUser.UserId, email = newUser.Email }));
                    
                    //return View("Users", null);
                }
                else return View ("UserExists");//если юзер с логином уже есть, то идем в представление с соответствующим сообщением
            }
            return View("FailRegistration");//ну а тут переход в представление, если вообще что-то пошло не так
        }

        [HttpGet]
        public ActionResult Logoff()
        {
            WebSecurity.Logout();
            return new RedirectResult(Url.Action("", ""));
        }

        [HttpGet]
        [Authorize(Roles ="Admins")]
        public ActionResult Admin()
        {
            var userId = 0;
            UserModel user = null;
            if (RouteData.Values.ContainsKey("id"))
            {
                if (int.TryParse(RouteData.Values["id"].ToString(), out userId))
                {
                    var userModel = MvcApplication.UserManager.GetUser(userId);
                    user = new UserModel
                    {
                        FullName = userModel.FullName,
                        Password = userModel.Password,
                        Email = userModel.Email,
                        Login = userModel.Login,
                        IsActive = userModel.IsActive,
                        IsApproved = userModel.IsApproved
                    };
                }
            }
            ViewBag.UserList = MvcApplication.UserManager.UserList();
            return View("Admin", user);
        }

        //Упражнение 11.2.2 шаг 7
        [HttpPost]
        [Authorize(Roles = "Admins")]
        public ActionResult Admin(UserModel user)
        {
            if (this.ModelState.IsValid)
            {
                MvcApplication.UserManager.SetUser(new UserModel
                {
                    //UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Login = user.Login,
                    Password = user.Password,
                    IsActive = user.IsActive,
                    IsApproved = user.IsApproved
                });
            }

            ViewBag.UserList = MvcApplication.UserManager.UserList();
            return View("Admin", null);
        }

        //этот метод вызывается после успешной регистрации нового пользователя
        //смотри строку return new RedirectResult(Url.Action("NewUser", "Account")) в методе public ActionResult Users(UserModel user)
        //тут нужно сгенерировать ключ валидации, сохранить его в БД [EShop].[dbo].[Registration] 
        //и отправить сообщение с этим ключом на эл. почту, которую юзер указал при регистрации  
        [HttpGet]
        public ActionResult NewUser(string email, int id)
        {
            string url = Request.Url.ToString();
            var url2 = url.Remove(url.IndexOf("NewUser"), url.Length - url.IndexOf("NewUser"));//немного быдлокодерства - изменяем имя текущего метода в Url на тот, который будет принимать переход по ссылке из почты
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("Для завершения регистрации перейдите по этой ссылке:\n");
            sb1.Append(url2).Append("CheckKey/").Append(id).Append("?key=");

            StringBuilder sb2 = new StringBuilder();
            for (int i = 0; i <15; i++)
            {
                sb2.Append(Convert.ToChar(rnd.Next(65,91)));//ключ будет из цифр и больших лат. букв
            }
            sb1.Append(sb2);
            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Timeout = 5000;
                    var msg = new MailMessage();
                    msg.To.Add(email);
                    var c = smtp.Credentials as NetworkCredential;
                    if (c != null)
                    {
                        msg.From = new MailAddress ("sergeikd@gmail.com");
                    }
                    msg.Subject = "Подтверждение регистрации на сайте EShop.by";
                    msg.Body = sb1.ToString();                    
                    smtp.Send(msg);
                }
            }
            catch (Exception ex)
            {
                //«тут что нить делаем, если почта не ушла»
            }
            //создаем в БД запись с ключем подтверждения и текущим временем
            dataService.StoreKey(new Entity.NewUser { Id = id, RegKey = sb2.ToString(), RegTime = DateTime.Now.ToString(CultureInfo.InvariantCulture) });

            return View("NewUser");
        }

        //этот метод должен примать данные, кторые были отправлены свежезарегенному пользователю для завершения регистрации
        //в той ссылке содержится имя этого метода, Id юзера  и ключ подтверждения
        [HttpGet]
        public ActionResult CheckKey( int? id, string key)
        {
            if (id != null)
            {
                //NewUser newUser = new NewUser();
                var newUser = dataService.GetKey((int)id);
                var userModel = MvcApplication.UserManager.GetUser((int)id);
                //проверка прошло ли 60 минут с момента отправки ключа регистрации на эл. почту юзера
                //var span = (DateTime.Now - DateTime.ParseExact(newUser.RegTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Minutes;
                if ((DateTime.Now - DateTime.ParseExact(newUser.RegTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Minutes < 60)
                {
                    //проверка совпадают ли ключи
                    if(key == newUser.RegKey)
                    {                        
                        userModel.IsApproved = true;//теперь пользователь зареген и подтвержден
                        MvcApplication.UserManager.SetUser(userModel);
                        dataService.DeleteRecord(userModel.UserId);//удаляем запись с ключом регистрации из БД 
                    }

                }
                else //если прошло, то удаляем аккаунт из БД UserEntity и Registration 
                {
                    MvcApplication.UserManager.DeleteAccount(userModel);
                    dataService.DeleteRecord(userModel.UserId);
                    return View("LinkExpired");//переход в представление с сообщением, что линк устарел
                }
            }
            else
                return View("FailRegistration");//тут тоже переход в представление, если вообще что-то пошло не так
            return View("SuccessRegistration");
        }
    }
}