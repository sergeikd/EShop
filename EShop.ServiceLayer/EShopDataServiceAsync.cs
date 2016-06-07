using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.Entity;
using System.Threading;

namespace EShop.ServiceLayer
{
    public partial class EShopDataService : IDataServiceAsync
    {

        public async Task<IEnumerable<Model>> GetRandomModelsAsync(int categoryID, int number)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetRandomModels(categoryID, number));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Model>> GetRandomModelsAsync(int categoryID, int number, CancellationToken сancellationToken)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetRandomModels(categoryID, number));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> GetCategoryListAsync()
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(GetCategoryList);
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> GetCategoryListAsync(CancellationToken сancellationToken)
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(GetCategoryList, сancellationToken);
            await task;
            return task.Result;
        }

        public async Task<bool> NewCategoryAsync(Category item)
        {
            var task = Task<bool>.Factory.StartNew(() => NewCategory(item));
            await task;
            return task.Result;
        }

        public async Task<bool> ChangeCategoryAsync(Category item)
        {
            var task = Task<bool>.Factory.StartNew(() => ChangeCategory(item));
            await task;
            return task.Result;
        }

        public async Task<bool> DeleteCategoryAsync(int itemId)
        {
            var task = Task<bool>.Factory.StartNew(() => DeleteCategory(itemId));
            await task;
            return task.Result;
        }

        public async Task<Category> GetCategoryAsync(int categoryID)
        {
            var task = Task<Category>.Factory.StartNew(() => GetCategory(categoryID));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> RootCategoriesAsync()
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(() => RootCategories);
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> PathToRootAsync(int categoryID)
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(() => PathToRoot(categoryID));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int categoryID)
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(() => GetSubCategories(categoryID));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Category>> GetAllSubCategoriesAsync(int categoryID)
        {
            var task = Task<IEnumerable<Category>>.Factory.StartNew(() => GetAllSubCategories(categoryID));
            await task;
            return task.Result;
        }

        public async Task<int> ModelsInCategoryAsync(int categoryID)
        {
            var task = Task<int>.Factory.StartNew(() => ModelsInCategory(categoryID));
            await task;
            return task.Result;
        }

        public async Task<int> ModelsInCategoryAsync(int categoryID, CancellationToken сancellationToken)
        {
            var task = Task<int>.Factory.StartNew(() => ModelsInCategory(categoryID), сancellationToken);
            await task;
            return task.Result;
        }

        public async Task<int> AddModelAsync(Model item)
        {
            var task = Task<int>.Factory.StartNew(() => AddModel(item));
            await task;
            return task.Result;
        }

        public async Task<bool> ChangeModelAsync(Model item)
        {
            var task = Task<bool>.Factory.StartNew(() => ChangeModel(item));
            await task;
            return task.Result;
        }

        public async Task<bool> DeleteModelAsync(int modelId)
        {
            var task = Task<bool>.Factory.StartNew(() => DeleteModel(modelId));
            await task;
            return task.Result;
        }

        public async Task<Model> GetModelAsync(int modelId)
        {
            var task = Task<Model>.Factory.StartNew(() => GetModel(modelId));
            await task;
            return task.Result;
        }

        public async Task<Delivery> GetDeliveryTypeAsync(int deliveryId)
        {
            var task = Task<Delivery>.Factory.StartNew(() => GetDeliveryType(deliveryId));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Availability>> GetAvailabilityTypesAsync()
        {
            var task = Task<IEnumerable<Availability>>.Factory.StartNew(() => GetAvailabilityTypes());
            await task;
            return task.Result;
        }
        public async Task<IEnumerable<Model>> GetModelsAsync(int categoryID)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetModels(categoryID));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Model>> GetModelsAsync(int categoryID, CancellationToken сancellationToken)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetModels(categoryID), сancellationToken);
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Model>> GetModelsAsync(int categoryID, int from, int pageSize)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetModels(categoryID, from, pageSize));
            await task;
            return task.Result;
        }

        public async Task<IEnumerable<Model>> GetModelsAsync(int categoryID, int from, int pageSize, CancellationToken сancellationToken)
        {
            var task = Task<IEnumerable<Model>>.Factory.StartNew(() => GetModels(categoryID, from, pageSize), сancellationToken);
            await task;
            return task.Result;
        }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
