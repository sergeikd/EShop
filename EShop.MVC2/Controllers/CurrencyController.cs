using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EShop.MVC2.Models;
using System.Web.Caching;

namespace EShop.MVC2.Controllers
{
    public class CurrencyController : Controller
    {
        //Метод для получения данных о курсах валют
        //Запрос сюда приходит из мастер-страницы /Views/Shared/_Layout.cshtml
        //(см. @{ Html.RenderAction("GetExRates", "Currency"); })
        //полученные данные отправляются в частичное представление /Views/Currency/GetExRates.cshtml для отрисовки HTML разметки

        // GET: /Currency/
        public ActionResult GetExRates()
        {
            IEnumerable<ExRateItem> items = null;
            var cache = HttpContext.Cache;
            if (cache["exRatesData"] == null)
            {
                var exRateClient = new ExRates.ExRatesSoapClient();
                var exRatesData = exRateClient.ExRatesDaily(DateTime.Now);
                //ViewBag.UpdateDate = DateTime.Now;//время получения курсов от НБРБ
                items = exRatesData.Tables[0].Select(
                            "Cur_Abbreviation = 'RUB' OR Cur_Abbreviation='USD' OR Cur_Abbreviation='EUR'").Select(row => new ExRateItem()
                            {
                                Exchange = row["Cur_Abbreviation"].ToString(),//код валюты: USD, EUR
                                Rate = (decimal)row["Cur_OfficialRate"]//значение курса
                            }).ToArray();

                cache.Insert("exRatesData", items, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(120), CacheItemPriority.Normal, null);
                cache.Insert("updateTime", DateTime.Now.ToUniversalTime(), null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(120), CacheItemPriority.Normal, null);
            }
            items = cache["exRatesData"] as IEnumerable<ExRateItem>;
            ViewBag.UpdateDate = cache["updateTime"];
            return PartialView("GetExRates", items);
        }
	}
}