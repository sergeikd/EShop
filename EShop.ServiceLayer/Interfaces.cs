using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.Entity;
using System.Threading;

namespace EShop.ServiceLayer
{
    // определение интерфейса службы бизнес-логики приложения 
    public interface IDataService : IDisposable 
    {
        ///<summary>
        /// Возвращает спиcок из number случайных моделей категории categoryId
        /// </summary>
        /// <param name = "categoryId">category ID</param>
        /// <param name = "number">quantity of random models</param>
        /// <returns></returns>
        IEnumerable<Model> GetRandomModels(int categoryId, int number);

        ///<summary>
        /// Возвращает спиcок всех категорий дерева в виде линейного массива
        /// </summary>
        /// <returns></returns>
        IEnumerable<Category> GetCategoryList();

        // базовые методы управления категориями    
        bool NewCategory(Category item);
        bool ChangeCategory(Category item);
        bool DeleteCategory(int itemId);
        Category GetCategory(int categoryId);
        IEnumerable<Category> RootCategories { get; }
        IEnumerable<Category> PathToRoot(int categoryId);
        IEnumerable<Category> GetSubCategories(int categoryId);
        IEnumerable<Category> GetAllSubCategories(int categoryId);
        int ModelsInCategory(int categoryId);

        //базовые методы управления моделями
        int AddModel(Model item);
        bool ChangeModel(Model item);
        bool DeleteModel(int modelId);
        Model GetModel(int modelId);
        bool UpdateModel(Model item);
        int GetMaxImageId();

        Delivery GetDeliveryType(int deliveryId);
        IEnumerable<Availability> GetAvailabilityTypes();
        IEnumerable<Delivery> GetDeliveryTypes();
        IEnumerable<Model> GetModels(int categoryID);
        IEnumerable<Model> GetModels(int categoryID, int from, int pageSize);

        bool StoreKey(NewUser newUser);
        NewUser GetKey(int userId);
        void DeleteRecord(int userId);

        bool SaveComment(Comments newComment);
        IEnumerable<Comments> GetComments(int productId);
        bool UpdateComment(Comments comment);
    }

    //Этот интерфейс декларирует контракт взаимодействия с бизнес-логикой в асинхронном режиме
    //асинхронные методы этого интерфейса сознательно вынесены в отдельную сущность, чтобы была
    //возможность отключить асинхронное поведение от сервиса приложения. Т.е. можно будет использовать
    //синхронную или асинхронную модель поведения сервиса приложения по отдельности или вместе
    public interface IDataServiceAsync : IDisposable
    {
        //каждый метод этого интерфейса имеет два варианта: один обычный, а другой с параметром CancellationToken сancellationToken
        //этот параметр нужен для организации отмены асинхронной задачи в случае ее неактуальности, истечении времени ожидания в вызывающем потоке
        //или в случае ошибки и невозможности ее продолжить
        //в некоторых статьях и учебных примерах методы с параметром CancellationToken не включены для экономии времени и места (скорее всего).
        //мы будем применять этот параметр (CancellationToken) к методам, где раельно можно получить превышение timeout ожидания или есть большая вероятность ошибки
        //в остальных методах интерфейса ограничимся обычной реализацией
        Task<IEnumerable<Model>> GetRandomModelsAsync(int categoryID, int number);
        Task<IEnumerable<Model>> GetRandomModelsAsync(int categoryID, int number, CancellationToken сancellationToken);

        Task<IEnumerable<Category>> GetCategoryListAsync();
        Task<IEnumerable<Category>> GetCategoryListAsync(CancellationToken сancellationToken);

        //базовые методы управления категориями
        Task<bool> NewCategoryAsync(Category item);
        Task<bool> ChangeCategoryAsync(Category item);
        Task<bool> DeleteCategoryAsync(int itemId);
        Task<Category> GetCategoryAsync(int categoryID);

        Task<IEnumerable<Category>> RootCategoriesAsync();// в асинхронной модели ASP.NET 4.5 свойства не очень удобно реализованы
        //поэтому заменяем свойство RootCategories на асинхронный метод
        Task<IEnumerable<Category>> PathToRootAsync(int categoryID);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int categoryID);
        Task<IEnumerable<Category>> GetAllSubCategoriesAsync(int categoryID);

        //в этом методе будет тяжелый рекурсивный запрос, поэтому добавим токен отмены асинхронной задачи в параметре
        Task<int> ModelsInCategoryAsync(int categoryID);
        Task<int> ModelsInCategoryAsync(int categoryID, CancellationToken сancellationToken);

        //базовые методы управления моделями
        Task<int> AddModelAsync(Model item);
        Task<bool> ChangeModelAsync(Model item);
        Task<bool> DeleteModelAsync(int modelId);
        Task<Model> GetModelAsync(int modelId);
        Task<Delivery> GetDeliveryTypeAsync(int deliveryId);
        Task<IEnumerable<Availability>> GetAvailabilityTypesAsync();

        //в этих методах тоже будут тяжелые рекурсивные операции с базой данных
        Task<IEnumerable<Model>> GetModelsAsync(int categoryID);
        Task<IEnumerable<Model>> GetModelsAsync(int categoryID, CancellationToken сancellationToken);
        Task<IEnumerable<Model>> GetModelsAsync(int categoryID, int from, int pageSize);
        Task<IEnumerable<Model>> GetModelsAsync(int categoryID, int from, int pageSize, CancellationToken сancellationToken);
    }


}