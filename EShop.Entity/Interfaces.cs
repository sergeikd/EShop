using System;
using System.Collections.Generic;

namespace EShop.Entity
{

    //	Интерфейс для доступа к репозиторию хранения данных о моделях.
    public interface IModelRepository : IDisposable 
    {
        ///<summary>
        /// Add model to repository
        /// </summary>
        /// <param name = "item">New model</param>
        /// <returns></returns>
        int AddModel(Model item);

        ///<summary>
        /// Update existing model
        /// </summary>
        /// <param name = "item">Model with changes</param>
        /// <returns></returns>
        bool UpdateModel(Model item);

        ///<summary>
        /// Delete model from repository
        /// </summary>
        /// <param name = "modelID"></param>
        /// <returns></returns>
        bool DeleteModel(int modelID);

        ///<summary>
        /// Find model by Id
        /// </summary>
        /// <param name = "modelID">Integer: model Id in DB</param>
        /// <returns></returns>
        Model GetModel(int modelID);

        ///<summary>
        /// Find delivery type by Id
        /// </summary>
        /// <param name = "ID">Short: delivery Id in DB [Delivery]</param>
        /// <returns></returns>
        Delivery GetDeliveryType(int deliveryId);

        ///<summary>
        /// Get all existing Availability types in  DB [Availability]
        /// </summary>
        /// <param name = ""></param>
        /// <returns></returns>
        IEnumerable<Availability> GetAvailabilityTypes();

        ///<summary>
        /// Get all existing Delivery types in  DB [Delivery]
        /// </summary>
        /// <param name = ""></param>
        /// <returns></returns>
        IEnumerable<Delivery> GetDeliveryTypes();

        ///<summary>
        /// Get all models in selected category
        /// </summary>
        /// <param name = "categoryId"></param>
        /// <returns></returns>
        IEnumerable<Model> GetModels(int categoryId);

        ///<summary>
        /// Get models by categoryId with paging
        /// </summary>
        /// <param name = "categoryId">is category</param>
        /// <param name = "pageNumber">page No</param>
        /// <param name = "pageSize">models per page</param>
        /// <returns></returns>
        IEnumerable<Model> GetModels(int categoryId, int pageNumber, int pageSize);

        ///<summary>
        /// Get Max ImageId in DataBase Models
        /// </summary>
        /// <returns></returns>
        int GetMaxImageId();
       
    }

    //	Интерфейс для доступа к репозиторию хранения данных о категориях
    public interface ICategoryRepository : IDisposable
    {
        bool AddCategory(Category item); //добавить новую категорию
        bool UpdateCategory(Category item); //обновить категорию в репозитории
        bool DeleteCategory(int itemId); //удалить категорию
        Category GetCategory(int categoryId);//найти категорию по Id
        IEnumerable<Category> RootCategories { get; }//список корневых категорий
        IEnumerable<Category> PathToRoot(int categoryId);//путь от выбранной категории categoryId по дереву вверх к корневой категории
        IEnumerable<Category> GetSubCategories(int categoryId);//список ближайших подкатегорий
        IEnumerable<Category> GetAllSubCategories(int categoryId);//список всех подкатегорий выбранной категории, независимо от глубины вложенности
        int CountModels(int categoryId);//количество моделей в выбранной категории, а так же во всех вложенных подкатегориях
    }

    //это мой интерфейс для работы с аккаунтами
    public interface IAccountRepository : IDisposable
    {
        //метод для сохранения ключа верификации при первичной регистрации
        bool StoreKey(NewUser newUser);

        //метод для чтения ключа если свежезарегенный юзер проходит по ссылке верификации
        NewUser GetKey(int userId);

        //метод для удаления записи из БД Registration
        void DeleteRecord(int userId);
    }

    //это мой интерфейс для работы с комментами
    public interface ICommentsRepository : IDisposable
    {
        bool SaveComment(Comments newComment);
        IEnumerable<Comments> GetComments(int productId);
        bool UpdateComment(Comments comment);

    }
}
