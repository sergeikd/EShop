namespace EShop.MVC2.Models
{
    //Сущность для хранения результатов поиска
    //Id используется для создания ссылки на товар
    //ImageId - для вывода иконки товара в результатах поиска
    //Description и Price понятно зачем
    public class SearchResult
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

    }
}