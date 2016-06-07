using EShop.Entity;
using EShop.ServiceLayer;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace EShop.MVC2.Controllers
{
    public class AdminController : Controller
    {
        private IDataService dataService;

        public AdminController(IDataService service)
        {
            dataService = service;
        }

        //Метод для обработки нажатия на кнопку "Изменить изображение" на странице описания модели
        //Из него идет перенаправление во View/Upload, где будет форма для выбора новой картинки 
        [Authorize(Roles = "Admins")]
        public ActionResult Upload(int? id, int? id2, int? id3)
        {
            ViewBag.FileId = (int)id;
            ViewBag.ModelId = (int)id2;
            //а вот тут костыль. Если не не запихнуть в ViewBag.Selected категорию товара, то панель навигации крашится.
            //хотя во View Model, откуда мы сюда приходим, в этом ViewBag.Selected еще значение есть.
            //а во View Upload, куда мы отсюда пойдем, его уже нету...  
            ViewBag.Selected = (int)id3;//
            return View("Upload");
        }

        [Authorize(Roles = "Admins")]
        public ActionResult CnahgeCategory(int? id, int? id2, int? id3)
        {
            ViewBag.FileId = (int)id;
            ViewBag.ModelId = (int)id2;

            ViewBag.Selected = (int)id3;
            return Redirect("~/Home/Model/" + (int)id);
        }

        //Метод для изменения текущих файлов изображения товара (и ico, и jpeg)
        //Из него идет возврат во View/Model.cshtml, где должна отобразиться новая картинка
        //Используется библиотека ImageResizer из NuGet 
        [Authorize(Roles = "Admins")]
        public ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files["file"];
            var imageId = Request.Form["imageId"];
            var modelId = Request.Form["modelId"];
            var req = Request.Files.Count;
            if (file != null)
            {
                //Declare a new dictionary to store the parameters for the image versions.
                var versions = new Dictionary<string, string>();

                var path = Server.MapPath("~/Content/Images/");

                //Define the versions to generate
                versions.Add("ico_", "maxwidth=100&format=jpg");
                versions.Add("img_", "maxwidth=400&format=jpg");

                //Generate each version
                foreach (var suffix in versions.Keys)
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);

                    //Let the image builder add the correct extension based on the output file type
                    ImageBuilder.Current.Build(
                        new ImageJob(
                            file.InputStream,
                            path + suffix + imageId,
                            new Instructions(versions[suffix]),
                            false,
                            true));
                }
            }
            return Redirect("~/Home/Model/"+ modelId);
        }

        //Метод для обработки клика по меню "Добавить товар" в панели админа
        //Сюда попадаем из частичного представления AdminPanel.cshtml (см.  @Html.ActionLink("Добавить товар", "AddGoods", "Admin"...)
        //Задача метода положить во ViewBag данные для выпадающих меню о доступности и способе доставки товара 
        //и передать во View AddGoods.cshtml пустую модель, или неполную для исправления выявленных ошибок
        [Authorize(Roles = "Admins")]
        public ActionResult AddGoods(Model model)
        {
            ViewBag.AvailabilityListModel = new SelectList(GetAvalabilityList(), "Id", "AvailabilityType");
            ViewBag.DeliveryListModel = new SelectList(GetDeliveryList(), "Id", "DeliveryType");
            return View("AddGoods", model);
        }

        //Метод для добавления в БД нового товара
        //Сюда попадаем из ~View/Home/AddGoods.cshtml
        //model должна получить данные из формы в той вьюхе        
        [HttpPost]
        [Authorize(Roles = "Admins")]
        public ActionResult AddModel(Model model)
        {
            List<string> error = new List<string>();
            if (Request.Form["newCategoryId"] != null)
                model.CategoryId = Int16.Parse(Request.Form["newCategoryId"]);
            else
                error.Add("Укажите категорию");
            if (Request.Form["Price"] != null)
            {
                try
                {
                    model.Price = Decimal.Parse(Request.Form["Price"].ToString().Replace(".", ","));// замена . на , на случай неправильного ввода цены
                }
                catch
                {
                    error.Add("Ошибка ввода цены");
                }               
            }                
            else
                error.Add("Введите цену");
            if (model.Title == null)
                error.Add("Введите краткое описание товара");
            if (model.Description == null)
                error.Add("Введите подробное описание товара");
            if(model.Warranty == 0)
                error.Add("Введите срок гарантии");
            if (error.Count > 0)
            {
                ViewBag.Error = error;
                ViewBag.AvailabilityListModel = new SelectList(GetAvalabilityList(), "Id", "AvailabilityType");
                ViewBag.DeliveryListModel = new SelectList(GetDeliveryList(), "Id", "DeliveryType");
                return View("AddGoods", model);
            }
                
            model.ImageId = dataService.GetMaxImageId() + 1;// присваиваем для ImageId значение, следующее после максимального, содержащегося в таблице Models базы
            var id = dataService.AddModel(model);
            return Redirect("~/Home/Model/" + model.Id); //переход на страницу редактирования добавленного товара
        }

        //Meтод для возврата списка всех возможных значений поля AvailabilityType найденных в БД [Availability]
        //Все как в контроллере Search, только тут без значений по дефолту
        //используется кэш
        List<Availability> GetAvalabilityList()
        {
            IEnumerable<Availability> avail;
            List<Availability> availabilityList = new List<Availability>();
            var cacheAvailabilityList = HttpContext.Cache;
            if (cacheAvailabilityList["availabilityListModel"] == null)
            {
                avail = dataService.GetAvailabilityTypes();
                foreach (var item in avail)
                {
                    availabilityList.Add(new Availability { Id = item.Id, AvailabilityType = item.AvailabilityType });
                    cacheAvailabilityList.Insert("availabilityListModel", availabilityList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
                }
            }
            availabilityList = cacheAvailabilityList["availabilityListModel"] as List<Availability>;
            return availabilityList;
        }

        //Meтод для возврата списка всех возможных значений поля DeliveryType найденных в БД [Delivery]
        //Все как в контроллере Search, только тут без значений по дефолту
        //используется кэш
        List<Delivery> GetDeliveryList()
        {
            IEnumerable<Delivery> delivery;
            List<Delivery> deliveryList = new List<Delivery>();
            var cacheDeliveryList = HttpContext.Cache;
            if (cacheDeliveryList["deliveryListModel"] == null)
            {
                delivery = dataService.GetDeliveryTypes();
                foreach (var item in delivery)
                {
                    deliveryList.Add(new Delivery { Id = item.Id, DeliveryType = item.DeliveryType });
                    cacheDeliveryList.Insert("deliveryListModel", deliveryList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
                }
            }
            deliveryList = cacheDeliveryList["deliveryListModel"] as List<Delivery>;
            return deliveryList;
        }
    }
}