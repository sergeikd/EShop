using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Entity;

namespace EShop.ServiceLayer
{
    public partial class EShopDataService : IDataService
    {
        private static readonly Exception NullProviderError; //   ******************** что это и зачем? при каких условиях возникнет этот эксепшн?****************************
        static EShopDataService()
        {
            NullProviderError = new Exception("EShopDataService: data provider not installed");
        }

        private readonly IModelRepository _modelRepository; 
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICommentsRepository _commentsRepository;

        public EShopDataService(IModelRepository modelRepository, ICategoryRepository categoryRepository, IAccountRepository accountRepository, ICommentsRepository commentsRepository)
        {
            _modelRepository = modelRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _commentsRepository = commentsRepository;
        }

        public void Dispose()
        {
            if (_modelRepository == null)
                _modelRepository.Dispose();
            if (_categoryRepository == null)
                _categoryRepository.Dispose();
            if (_accountRepository == null)
                _accountRepository.Dispose();
            if (_commentsRepository == null)
                _commentsRepository.Dispose();
        }

        //метод IDataService.GetRandomModels пример совместного использования репозиториев
        public IEnumerable<Model> GetRandomModels(int categoryId, int number)
        {
            if (_modelRepository == null) throw NullProviderError;
            if (_categoryRepository == null) throw NullProviderError;
            var modelsCount = _categoryRepository.CountModels(categoryId);
            var rnd = new Random();
            var modelList = _modelRepository.GetModels(categoryId).ToArray();
            var randomModels = new List<Model>();
            var index = 0;
            while (index < number)
            {
                var n = rnd.Next(0, modelsCount);
                var model = modelList[n];
                if (model == null) continue;
                if (randomModels.Any(m => m.Id == model.Id)) continue; 
                randomModels.Add(modelList[n]);
                index++;
            }
            return randomModels;
        }

        // пример собственной логики приложения - обход дерева категорий и оставление линейного списка всех категорий
        public IEnumerable<Category> GetCategoryList()
        {
            var list = new List<Category>();
            foreach (var c in RootCategories)
            {
                list.Add(c);
                list.AddRange(GetAllSubCategories(c.Id));
            }
            return list.OrderBy(c => c.Id).Select(c => c);
        }

        //далее идет упрощенная бизнес-логика, на вид просто прокси-класс для передачи вызова в репозитории
        //в коммерческих проектах все гораздо сложнее 
        public bool NewCategory(Category item)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.AddCategory(item);
        }

        public bool DeleteCategory(int itemId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.DeleteCategory(itemId);
        }

        public Category GetCategory(int categoryId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.GetCategory(categoryId);
        }

        public IEnumerable<Category> RootCategories
        {
            get
            {
                if (_categoryRepository == null) throw NullProviderError;
                return _categoryRepository.RootCategories;
            }
        }

        public bool DeleteModel(int modelId)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.DeleteModel(modelId);
        }

        public Model GetModel(int modelId)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetModel(modelId);
        }

        public Delivery GetDeliveryType(int deliveryId)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetDeliveryType(deliveryId);
        }
        public IEnumerable<Availability> GetAvailabilityTypes()
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetAvailabilityTypes();
        }
        public IEnumerable<Delivery> GetDeliveryTypes()
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetDeliveryTypes();
        }

        public bool ChangeCategory(Category item)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.UpdateCategory(item);
        }

        public IEnumerable<Category> PathToRoot(int categoryId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.PathToRoot(categoryId);
        }

        public IEnumerable<Category> GetSubCategories(int categoryId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.GetSubCategories(categoryId);
        }

        public IEnumerable<Category> GetAllSubCategories(int categoryId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.GetAllSubCategories(categoryId);
        }

        public int ModelsInCategory(int categoryId)
        {
            if (_categoryRepository == null) throw NullProviderError;
            return _categoryRepository.CountModels(categoryId);
        }

        public int AddModel(Model item)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.AddModel(item);
        }

        public bool ChangeModel(Model item)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.UpdateModel(item);
        }

        public IEnumerable<Model> GetModels(int categoryID)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetModels(categoryID);
        }

        public IEnumerable<Model> GetModels(int categoryID, int from, int pageSize)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetModels(categoryID, from, pageSize);
        }

        public int GetMaxImageId()
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.GetMaxImageId();
        }
        public bool UpdateModel(Model item)
        {
            if (_modelRepository == null) throw NullProviderError;
            return _modelRepository.UpdateModel(item);
        }
        //Мои доделки с интерфейсами для Account 
        public bool StoreKey(NewUser newUser)
        {
            if (_accountRepository == null) throw NullProviderError;
            return _accountRepository.StoreKey(newUser);
        }

        public NewUser GetKey(int userId)
        {
            if (_accountRepository == null) throw NullProviderError;
            return _accountRepository.GetKey(userId);
        }

        public void DeleteRecord(int userId)
        {
            if (_accountRepository == null) throw NullProviderError;
            _accountRepository.DeleteRecord(userId);
        }

        //Мои доделки с интерфейсами для Comments 
        public bool SaveComment(Comments newComment)
        {
            if (_commentsRepository == null) throw NullProviderError;
            return _commentsRepository.SaveComment(newComment);
        }
        public IEnumerable<Comments> GetComments(int productId)
        {
            if (_commentsRepository == null) throw NullProviderError;
            return _commentsRepository.GetComments(productId);
        }

        public bool UpdateComment(Comments comment)
        {
            if (_commentsRepository == null) throw NullProviderError;
            return _commentsRepository.UpdateComment(comment);
        }
    }
}
