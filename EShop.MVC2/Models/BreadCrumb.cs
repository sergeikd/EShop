
namespace EShop.MVC2.Models
{
    //Сущность для построения BreadCrumb
    //Поле Text показывает название раздела веб-приложения
    //Поле Link содержит ссылку на страницу сайта 
    public class BreadCrumb
    {
        public string Text { get; set; }
        public string Link { get; set; }
    }
}