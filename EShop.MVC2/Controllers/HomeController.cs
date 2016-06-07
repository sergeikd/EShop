using EShop.Entity;
using EShop.MVC2.Models;
using EShop.ServiceLayer;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Mvc;

namespace EShop.MVC2.Controllers
{
    public class HomeController : Controller
    {
        private IDataService dataService;

        public HomeController(IDataService service)
        {
            dataService = service;
        }

        //Метод контроллера для обработки запросов URL вида ~/Home/Market/id/id2
        //если id отсутствует, то ViewBag.Selected будет null и в представлении Views/Home/Market.cshtml отобразится простое приветствие
        //если id есть, а id2 нет, то в представлении отобразятся первые товары из категории id
        //если есть и id, и id2, то отображаются товары из категории id и страницы списка id2
        public ActionResult Market(int? id, int? id2 = null)
        {
            int modelsPerPage = Int16.Parse(WebConfigurationManager.AppSettings["ModelsPerPage"]);//чтение из web.config количества моделей на страницу (см <add key="ModelsPerPage" value="16" />)
            Model[] models = new Model[modelsPerPage];
            if (id.HasValue && !id2.HasValue)
            {
                ViewBag.Selected = id.Value;
                models = dataService.GetModels((int)id).ToArray().Take(modelsPerPage).ToArray();
                ViewBag.ModelsInCategory = dataService.GetModels((int)id).Count();//сколько всего моделей в выбранной категории
                ViewBag.ModelsPerPage = modelsPerPage;
            }
            if (id != null && id2 != null)
            {
                models = dataService.GetModels((int)id, (int)id2, modelsPerPage).ToArray();
                ViewBag.QuantityModels = dataService.GetModels((int)id).Count();//сколько всего моделей в выбранной категории
                ViewBag.Selected = id.Value;
                ViewBag.ModelsPerPage = modelsPerPage;
                ViewBag.ModelsInCategory = dataService.GetModels((int)id).Count();
                ViewBag.Page = (int)id2;//какая по номеру страница была запрошена
            }
            return PartialView("Market", models);

            //if (id.HasValue) ViewBag.Selected = id.Value;
            //var models = dataService.GetModels(id.HasValue ? id.Value : 8232).Take(20).ToArray();
            //return PartialView("Market", models);
        }

        //метод готовит данные для частичного представления /View/Shared/Navigate.cshtml
        //во ViewData собираются категории необходимых уровней
        //затем в представлении  эти данные вынимаются и отрисовывается дерево навигации
        //с выделением выбранной категории 
        [ChildActionOnly]
        public ActionResult Categories(int? id)
        {
            if (!id.HasValue)
            {
                TreeBuilder();//этот метод должен занести во ViewData только корневые категории
            }
            else
            {
                var path = dataService.PathToRoot((int)id).ToArray();//находим путь к корневой категории
                if (path.Length == 0)
                {
                    id = null;//если в пути ничего нет, 
                    TreeBuilder();//значит адрес ошибочный и показываем только корневые категории
                }

                if (path.Length == 1)//если в пути только один элемент, значит это тоже корневая категория     
                {
                    TreeBuilder(id);//готовим для представления Navigate корневые категрии и подкатегории первого уровня в соответствии с URL id 
                    ViewBag.IdLevel0 = path[0].Id;
                }

                if (path.Length == 2)
                {
                    TreeBuilder(path[0].Id, path[1].Id);//этот метод должен занести во ViewData корневые категории и подкатегорию первого уровня
                    ViewBag.IdLevel0 = path[0].Id;
                    ViewBag.IdLevel1 = path[1].Id;
                }

                if (path.Length == 3)
                {
                    TreeBuilder(path[0].Id, path[1].Id, path[2].Id);//этот метод должен занести во ViewData корневые катеории и подкатегории первого и второго уровня

                    ViewBag.IdLevel0 = path[0].Id;
                    ViewBag.IdLevel1 = path[1].Id;
                    ViewBag.IdLevel2 = path[2].Id;
                }
            }
            return PartialView("Navigate");
        }

        //Метод контроллера для обработки запросов URL вида ~/Home/Model/id
        //данные о модели model отправляются в представление  /View/Home/Market.cshtml
        //все остальные данные нужны для отрисовки дерева до родительской категории выбранного товара 
        public ActionResult Model(int? id)
        {
            //ViewBag.Selected = id.Value;
            var model = dataService.GetModel((int)id);
            if (model == null)
                return Redirect("~");//если завпрашиваемой модели нет, то возврат в начало сайта
            //все что ниже нужно для отрисовки дерева категорий в левой панели (аналогично методу Categories) 
            var path = dataService.PathToRoot((int)model.CategoryId).ToArray();
            if (path.Length == 1)//если в пути только один элемент, значит это тоже корневая категория
            {
                TreeBuilder(path[0].Id);
                ViewBag.IdLevel0 = path[0].Id;
                ViewBag.Selected = path[0].Id;
            }

            if (path.Length == 2)
            {
                TreeBuilder(path[0].Id, path[1].Id);
                ViewBag.IdLevel0 = path[0].Id;
                ViewBag.IdLevel1 = path[1].Id;
                ViewBag.Selected = path[1].Id;
            }

            if (path.Length == 3)
            {
                TreeBuilder(path[0].Id, path[1].Id, path[2].Id);

                ViewBag.IdLevel0 = path[0].Id;
                ViewBag.IdLevel1 = path[1].Id;
                ViewBag.IdLevel2 = path[2].Id;
                ViewBag.Selected = path[2].Id;
            }
            ViewBag.Comments = dataService.GetComments(model.Id); //Комментарии к товару из БД Comments
            //Далее готовятся данные для выпадающих списков полей товаров
            //Они будут нужны если юзер - админ
            //SelectList availability = new SelectList(GetAvalabilityList(), "Id", "AvailabilityType");// model.AvailabilityId.ToString());
            //ViewBag.AvailabilityListModel = availability;
            ViewBag.AvailabilityListModel = new SelectList(GetAvalabilityList(), "Id", "AvailabilityType");
            ViewBag.DeliveryListModel = new SelectList(GetDeliveryList(), "Id", "DeliveryType");
            return View("Model", model);
        }

        [ChildActionOnly]
        public ActionResult BreadCrumb(int? id)
        {
            List<BreadCrumb> breadCrumbTrail = new List<BreadCrumb>() { new BreadCrumb { Text = "Главная", Link = "/" } };//поле навигации для contentPanel c указанием на главную страницу для первого элемента
            if (id.HasValue)
            {
                var path = dataService.PathToRoot((int)id);
                foreach (var level in path)
                    breadCrumbTrail.Add(new BreadCrumb
                    {
                        Text = dataService.GetCategory(level.Id).Name,  //название категории
                        Link = level.Id.ToString()
                    });                   // линк на категорию
            }
            return PartialView(breadCrumbTrail);
        }

        // Метод для построения дерева категорий с учетом подкатегорий разной глубины (Root > Level1 > Level2)
        public void TreeBuilder(int? id1 = null, int? id2 = null, int? id3 = null)
        {
            List<CategoryList> Root = new List<CategoryList>();
            List<CategoryList> Level1 = new List<CategoryList>();
            List<CategoryList> Level2 = new List<CategoryList>();
            IEnumerable<Category> root = null;
            var cache = HttpContext.Cache;
            if (cache["root"] == null)
            {
                root = dataService.RootCategories;
                cache.Insert("root", root, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            root = cache["root"] as IEnumerable<Category>;//dataService.RootCategories;

            if (id1 == null && id2 == null && id3 == null)//если пришел запрос без параметров, то набиваем в ViewData только корневые категории
            {
                foreach (var item in root)
                {
                    Root.Add(new CategoryList { Id = item.Id, Name = item.Name });
                }
                Level1 = null;
                Level2 = null;
                ViewData["Root"] = Root;
                ViewData["Level1"] = Level1;
                ViewData["Level2"] = Level2;
            }

            if (id1 != null && id2 == null && id3 == null)//если пришел запрос с параметром корневой категории, то набиваем в ViewData корневые категории с подкатегорией
            {
                foreach (var item in root)
                    Root.Add(new CategoryList { Id = item.Id, Name = item.Name });

                //генерим лист подкатегорий 1-го уровня для выбранной корневой категории 
                var subCat = dataService.GetSubCategories((int)id1);
                foreach (var item in subCat)
                    Level1.Add(new CategoryList { Id = item.Id, Name = item.Name });

                Level2 = null;
                ViewData["Root"] = Root;
                ViewData["Level1"] = Level1;
                ViewData["Level2"] = Level2;
            }

            if (id1 != null && id2 != null && id3 == null)//если пришел запрос с параметром для подкатегории первого уровня, то набиваем в ViewData корневые категории с подкатегориями первого уровня
            {
                foreach (var item in root)
                    Root.Add(new CategoryList { Id = item.Id, Name = item.Name });

                var subCat = dataService.GetSubCategories((int)id1);
                foreach (var item in subCat)
                    Level1.Add(new CategoryList { Id = item.Id, Name = item.Name });

                subCat = dataService.GetSubCategories((int)id2);
                foreach (var item in subCat)
                    Level2.Add(new CategoryList { Id = item.Id, Name = item.Name });

                ViewData["Root"] = Root;
                ViewData["Level1"] = Level1;
                ViewData["Level2"] = Level2;
            }

            if (id1 != null && id2 != null && id3 != null)//если пришел запрос с параметром для подкатегории второго уровня, то набиваем в ViewData корневые категории с подкатегориями 1-го и 2-го уровней
            {
                foreach (var item in root)
                    Root.Add(new CategoryList { Id = item.Id, Name = item.Name });

                var subCat = dataService.GetSubCategories((int)id1);
                foreach (var item in subCat)
                    Level1.Add(new CategoryList { Id = item.Id, Name = item.Name });

                subCat = dataService.GetSubCategories((int)id2);
                foreach (var item in subCat)
                    Level2.Add(new CategoryList { Id = item.Id, Name = item.Name });

                ViewData["Root"] = Root;
                ViewData["Level1"] = Level1;
                ViewData["Level2"] = Level2;
            }
        }

        //Метод для приема нового комментария из представления View/Home/Model.cshtml
        //После обработки вызывается метод public ActionResult Model(int? id) для отображения результата
        [HttpPost]
        [Authorize]
        public ActionResult Comment (string author, int model, string comment)
        {
            if(comment !="")
            {
                var user = MvcApplication.UserManager.GetUser(author);
                Comments commentNew = new Comments
                                           {UserName = user.FullName,
                                            UserId = user.UserId,
                                            ProductId = model,
                                            Comment = comment,
                                            CommentTime = DateTime.Now.ToString(CultureInfo.InvariantCulture)};
                
                var existComments = dataService.GetComments(model).ToList();
                if (existComments.Count() == 0)                                               
                    dataService.SaveComment(commentNew);//если комментариев к товару еще нет, то просто сохраняем его
                else                                    //а если есть, то проверяем отписывался ли уже этот юзер там
                {
                    if (existComments.Exists(x => x.UserId == user.UserId))
                        dataService.UpdateComment(commentNew);  //если отписывался, то обновляем его коммент
                    else
                        dataService.SaveComment(commentNew);    //если не отписывался, то добавляем коммент
                }
            }
            return Redirect("~/Home/Model/"+model);
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

        //Метод для сохранения отредактированных данных о товаре
        //Сюда попадаем из представления View/Home/Model.cshtml при нажатии кнопки Сохранить
        //и это может сделать только админ 
        [HttpPost]
        [Authorize(Roles = "Admins")]
        public ActionResult ModelEdit(int modelId)
        {
            var model = dataService.GetModel(modelId); //читаем старые данные о модели по ее modelId
            //обновляем поля, пришедшие из формы редактирования
            model.Delivery.Id = Int16.Parse(Request.Form["DeliveryId"]);
            model.Availability.Id = Int16.Parse(Request.Form["AvailabilityId"]);
            model.Description = Request.Form["Description"];
            //model.Price = Decimal.Parse(Request.Form["Price"].Replace(".", ","));
            model.Title = Request.Form["Title"];
            model.Warranty = Int16.Parse(Request.Form["Warranty"]);
            List<string> error = new List<string>();
            if (Request.Form["newCategoryId"] != null)
                model.CategoryId = Int16.Parse(Request.Form["newCategoryId"]);
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
            if (model.Title == null || model.Title=="")
                error.Add("Введите краткое описание товара");
            if (model.Description == null || model.Description == "")
                error.Add("Введите подробное описание товара");
            if (model.Warranty == 0)
                error.Add("Введите срок гарантии");
            if (error.Count > 0)
            {
                ViewBag.Error = error;
                ViewBag.AvailabilityListModel = new SelectList(GetAvalabilityList(), "Id", "AvailabilityType");
                ViewBag.DeliveryListModel = new SelectList(GetDeliveryList(), "Id", "DeliveryType");
                return View("Model", model);
            }
            dataService.UpdateModel(model);
            //refreshCacheModelId();//Обновить кэш после изменения товара
            return Redirect("~/Home/Model/"+modelId);
        }

        private void refreshCache()
        {
            var tree = new List<Layer1>();
            var cache = HttpContext.Cache;
            var layer1 = dataService.RootCategories;
            foreach (var item1 in layer1)
            {
                var layer2 = dataService.GetSubCategories(item1.Id);
                var tree2 = new List<Layer2>();
                foreach (var item2 in layer2)
                {
                    var layer3 = dataService.GetSubCategories(item2.Id);
                    var tree3 = new List<Layer3>();
                    foreach (var item3 in layer3)
                    {
                        tree3.Add(new Layer3 { CategoryName3 = item3.Name, CategoryId3 = item3.Id });
                    }
                    tree2.Add(new Layer2 { CategoryName2 = item2.Name, CategoryId2 = item2.Id, Categories2 = tree3 });
                }
                tree.Add(new Layer1 { CategoryName1 = item1.Name, CategoryId1 = item1.Id, Categories1 = tree2 });
            }
            cache.Insert("WholeTree", tree, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
        }
        //Метод для удаления товара из БД
        //Сюда попадаем из представления View/Home/Model.cshtml при нажатии кнопки Удалить
        //и это может сделать только админ 
        [Authorize(Roles = "Admins")]
        public ActionResult ModelDelete(int? id)
        {
            try
            {
                Model model = dataService.GetModel((int) id);
                System.IO.File.Delete("~/Content/Images/ico_" + model.ImageId + ".jpg");
                System.IO.File.Delete("~/Content/Images/img_" + model.ImageId + ".jpg");
            }
            catch(Exception ex)
            {

            }
            dataService.DeleteModel((int)id);
            return Redirect("~/Home/Market");
        }


        //Метод ImageBuilder берется из приложения ImageResizer (см. using ImageResizer), а оно из Nuget
        [HttpPost]
        [Authorize(Roles = "Admins")]
        public ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files["file"];
            var req = Request.Files.Count;
            if (file != null)
            {
                //Declare a new dictionary to store the parameters for the image versions.
                var versions = new Dictionary<string, string>();

                var path = Server.MapPath("~/Content/Images/");

                //Define the versions to generate
                versions.Add("_ico", "maxwidth=100&maxheight=100&format=jpg");
                versions.Add("_img", "maxwidth=400&maxheight=400&format=jpg");

                //Generate each version
                foreach (var suffix in versions.Keys)
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);

                    //Let the image builder add the correct extension based on the output file type
                    ImageBuilder.Current.Build(
                        new ImageJob(
                            file.InputStream,
                            path + suffix + file.FileName,
                            new Instructions(versions[suffix]),
                            false,
                            true));
                }
            }
            return Redirect("~/Home/Market");
        }

        //Метод для подготовки модели tree, собержащей полное дерево категорий 
        //Эта модель передается в частичное представление WholeTree.cshtml, а затем отрисовывается на вьюхе Model.cshtml
        //[Authorize(Roles = "Admins")]
        public ActionResult WholeTree()
        {
            var tree = new List<Layer1>();
            var cache = HttpContext.Cache;
            if (cache["WholeTree"] == null)
            {
                var layer1 = dataService.RootCategories;
                foreach (var item1 in layer1)
                {
                    var layer2 = dataService.GetSubCategories(item1.Id);
                    var tree2 = new List<Layer2>();
                    foreach (var item2 in layer2)
                    {
                        var layer3 = dataService.GetSubCategories(item2.Id);
                        var tree3 = new List<Layer3>();
                        foreach (var item3 in layer3)
                        {
                            tree3.Add(new Layer3 { CategoryName3 = item3.Name, CategoryId3 = item3.Id });
                        }
                        tree2.Add(new Layer2 { CategoryName2 = item2.Name, CategoryId2 = item2.Id, Categories2 = tree3 });
                    }
                    tree.Add(new Layer1 { CategoryName1 = item1.Name, CategoryId1 = item1.Id, Categories1 = tree2 });
                }
                cache.Insert("WholeTree", tree, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1));
            }
            tree = cache["WholeTree"] as List<Layer1>;
            return PartialView("WholeTree", tree);
        }

        //Метод для подготовки модели int (список всех ImageId что есть в БД) для передачи в частичное представление UsedId.chtml ,
        //а затем отрисовывается на вьюхе AddGoods.cshtml
        public ActionResult UsedImagesId()
        {
            var maxId = dataService.GetMaxImageId();
            return PartialView("UsedImgId", maxId);
        }
    }
}