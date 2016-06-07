using EShop.Entity;
using EShop.MVC2.Models;
using EShop.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;

namespace EShop.MVC2.Controllers
{
    public class SearchController : Controller
    {
        private IDataService dataService;

        public SearchController (IDataService service)
        {
            dataService = service;
        }

        //Метод для сбора данных для выпадающих меню в форме поиска:
        // - список категорий товаров
        // - варианты наличия товара
        //Запрос сюда приходит из мастер-страницы /Views/Shared/_Layout.cshtml
        //(см.@{ Html.RenderAction("Index", "Search"); })
        //полученные данные отправляются в частичное представление /Views/Search/Searching.cshtml для отрисовки HTML разметки
        public ActionResult Index()
        {
            //для начала достанем все корневые категории, как и для левой панели с навигацией
            IEnumerable<Category> root = null;
            var cacheRoot = HttpContext.Cache;
            if (cacheRoot["root"] == null)
            {
                root = dataService.RootCategories;
                cacheRoot.Insert("root", root, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            root = cacheRoot["root"] as IEnumerable<Category>;
            //далее берем из кэша или генерим список только из Category.Name и Category.Id
            //он нужен, чтоб не передавать лишнюю инфу в DropDownList через ViewBag
            //сразу закладываем первый элемент для возможности поиска по всем категориям по умолчанию
            List<CategoryList> categoryList = new List<CategoryList>();
            var cacheCategoryList = HttpContext.Cache;
            if (cacheCategoryList["categoryList"] == null)
            {
                categoryList.Add(new CategoryList { Id = 0, Name = "Все категории" });
                foreach (var item in root)
                    categoryList.Add( new CategoryList { Id = item.Id, Name = item.Name });

                cacheCategoryList.Insert("categoryList", categoryList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            categoryList = cacheCategoryList["categoryList"] as List<CategoryList>;

            SelectList categories = new SelectList(categoryList, "Id", "Name", "0");
            ViewBag.CategoriesList = categories;

            //аналогично берем из кэша или генерим список <Availability> AvailabilityType и Id
            var availabilityList = GetAvalabilityList();
            SelectList availability = new SelectList(availabilityList, "Id", "AvailabilityType", "0");
            ViewBag.AvailabilityList = availability;

            return PartialView("Searching");
        }
        
        //[HttpGet]
        public ActionResult SearchRequest()
        {
            List<SearchResult> resultList = new List<SearchResult>();//список с результаиами совпадений запроса с Model.Description
            //тут начинается разбор запроса поиска
            string searchQuery = Request.QueryString["search_query"];
            if (searchQuery != "")
            {
                byte availability = Byte.TryParse(Request.QueryString["availability"], out availability) ? availability : (byte)0;//если в поле "availability" пришла какая-то ерунда, то заносим туда 0 
                var priceFrom = TryParseNullable(Request.QueryString["from"]);
                var priceTo = TryParseNullable(Request.QueryString["to"]);
                //var searchWords = searchQuery.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();//делим поисковый запрос на отдельные слова
                //ViewBag.SearchWords = searchWords; //и отправлем этот массив для SerachResult.cshtml
                ViewBag.SearchQuery = searchQuery;
                int category = Int32.TryParse(Request.QueryString["category"], out category) ? category : 0;//если вдруг в поле "category" пришла какая-то ерунда, то заносим туда 0
                if (category == 0)//Если категория не указана, то придется искать ВСЕ товары
                {
                    IEnumerable<Category> root = null;
                    var cacheRoot = HttpContext.Cache;
                    if (cacheRoot["root"] == null)
                    {
                        root = dataService.RootCategories;
                        cacheRoot.Insert("root", root, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
                    }
                    root = cacheRoot["root"] as IEnumerable<Category>;
                    foreach (var item in root)
                        //AddRange тут потому что складываются списки
                        resultList.AddRange(GetSearchResultList(availability, priceFrom, priceTo, searchQuery, item.Id));
                }
                else//тут поиск по отдельной выбранной категории
                    resultList.AddRange(GetSearchResultList(availability, priceFrom, priceTo, searchQuery, category));        
                //foreach (var word in searchWords)
                //{

                //}
            }
            //Далее создаем новый список из неповторяющихся элементов списка resultList
            //почему-то метод Distinct() не работал, пришлось делать ручками
            List<SearchResult> finalList = new List<SearchResult>();
            List<int> resultListId = new List<int>();
            foreach (var item in resultList)
            {                
                if (!resultListId.Contains(item.Id))
                {
                    finalList.Add(item);
                    resultListId.Add(item.Id);
                }
            }
            //сортировка списка по количеству найденных совпадений
            //пришлось сделать еще один список :(
            //var fnlList = finalList.OrderBy(x => x.Id);
            return PartialView("SearchResult", finalList);
        }

        //Метод для создания списка результата поиска
        List<SearchResult> GetSearchResultList(int availability, decimal? priceFrom, decimal? priceTo, string word, int categoryId)
        {
            List<SearchResult> resultList = new List<SearchResult>();
            var avalabilityList = GetAvalabilityList();
            var models = dataService.GetModels(categoryId);
            foreach (var m in models)
            {
                if (availability == 0 || availability == m.AvailabilityId)//если критерий наличия подходит, то разбираем запрос дальше 
                {
                    if ((priceFrom == null || priceFrom <= m.Price) && (priceTo == null || priceTo >= m.Price))//если критерий цены подходит, то наконец сравниваем строки
                    {
                        if (m.Description.Contains(word))
                            resultList.Add(new SearchResult
                                    {
                                        Description = m.Description,
                                        Id = m.Id,
                                        ImageId = m.ImageId,
                                        Price = m.Price
                                    });
                    }
                }
            }
            return resultList;
        }
        //Метод для проверки наличия в описании товара слова из поискового запроса
        bool  FindString (string searchQuery, string description)
        {
            bool result = false;
            var words = description.Split(' ').ToArray();
            if (words.Where(x => x.Contains(searchQuery)).Count() > 0)//есть ли совпадение (полное или частичное) searchQuery в массиве words
                result = true;
            return result;
        }

        //Meтод для возврата списка всех возможных значений поля AvailabilityType найденных в БД [Availability]
        //В спиок заносится первый элемент  Id = 0, AvailabilityType = "Любое", которого нет в БД, но он нужен для дефолтного элемента в выпадающем списке 
        //используется кэш
        List<Availability> GetAvalabilityList()
        {
            IEnumerable<Availability> avail;
            List<Availability> availabilityList = new List<Availability>();
            var cacheAvailabilityList = HttpContext.Cache;
            if (cacheAvailabilityList["availabilityList"] == null)
            {
                avail = dataService.GetAvailabilityTypes();
                availabilityList.Add(new Availability { Id = 0, AvailabilityType = "Любое" });
                foreach (var item in avail)
                {
                    availabilityList.Add(new Availability { Id = item.Id, AvailabilityType = item.AvailabilityType });
                    cacheAvailabilityList.Insert("availabilityList", availabilityList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
                }
            }
            availabilityList = cacheAvailabilityList["availabilityList"] as List<Availability>;
            return availabilityList;
        }

        //Метод парсинга строки в число
        //Выдает nullable тип - это нужно если в поле цены пользователь введет какую-то фигню
        decimal? TryParseNullable(string val)
        {
            decimal outValue;
            return Decimal.TryParse(val, out outValue) ? (decimal?)outValue : null;
        }


    }
}