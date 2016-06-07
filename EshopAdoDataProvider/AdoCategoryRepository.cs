using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EShop.Entity;
using EshopAdoDataProvider;

namespace EShopAdoDataProvider
{
    public class AdoCategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public AdoCategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// добавить новую категорию
        /// </summary>
        /// <param name="item">новая категория</param>
        /// <returns></returns>
        public bool AddCategory(Category item)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_Add]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 150)).Value = item.Name;
                        cmd.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int)).Value = item.ParentId;
                        cmd.Parameters.Add(new SqlParameter("@ImageID", SqlDbType.Int)).Value = item.ImageId;
                        cmd.Parameters.Add(new SqlParameter("@return_value", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue;
                        connect.Open();
                        var result = cmd.ExecuteNonQuery();
                        var value = cmd.Parameters["@return_value"].Value;
                        if (value != null)
                            item.Id = (int)value;
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AddCategory method failed.", ex);
            }
        }

        public bool UpdateCategory(Category item)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_Update]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = item.Id;
                        cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 150)).Value = item.Name;
                        cmd.Parameters.Add(new SqlParameter("@ParentID", SqlDbType.Int)).Value = item.ParentId;
                        cmd.Parameters.Add(new SqlParameter("@ImageID", SqlDbType.Int)).Value = item.ImageId;
                        //cmd.Parameters.Add(new SqlParameter("@return_value", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue;
                        connect.Open();
                        var result = cmd.ExecuteNonQuery();
                        //var value = cmd.Parameters["@return_value"].Value;
                        //if (value != null)
                        //    item.Id = (int)value;
                        //return result > 0;
                        //if (result >= 0) return true;
                        //else return false;
                        return (result > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Category_Update method failed.", ex);
            }
            //return true;
        }

        /// <summary>
        /// Удаление заданной категории
        /// </summary>
        /// <param name="itemId">код категории для удаления</param>
        /// <returns></returns>
        public bool DeleteCategory(int itemId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_DeleteCategory]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = itemId;
                        connect.Open();
                        //метод SqlCommand.ExecuteScalar возвращает только одно значение как результат запроса
                        //это может быть число, строка, дата, byte[] или еще что-то, что можно сохранить в одну переменную
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            return (int)result > 0;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DeleteCategory method failed.", ex);
            }
        }

        /// <summary>
        /// Возвращает полную информацию о категории по коду
        /// </summary>
        /// <param name="categoryId">код категории в БД</param>
        /// <returns></returns>
        public Category GetCategory(int categoryId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_GetCategory]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = categoryId;
                        connect.Open();
                        //здесь опять используем модификацию SqlCommand, которая заставляет сервер возвращать
                        //только одну строку в result set
                        using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            //reader.ToCategory() - вызов расширяющего метода для класса SqlDataReader
                            //сам метод см. в классе DataReaderEx этого проекта
                            return reader.Read() ? reader.ToCategory() : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCategory method failed.", ex);
            }
        }

        /// <summary>
        /// Свойство - список корневых категорий
        /// </summary>
        public IEnumerable<Category> RootCategories
        {
            get
            {
                try
                {
                    using (var connect = new SqlConnection(_connectionString))
                    {
                        using (var cmd = new SqlCommand("[Category_GetRootCategories]", connect))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            connect.Open();
                            using (var reader = cmd.ExecuteReader())
                            {
                                var list = new List<Category>();
                                while (reader.Read())
                                {
                                    list.Add(reader.ToCategory());
                                }
                                return list;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("RootCategories method failed.", ex);
                }
            }
        }

        /// <summary>
        /// Список категорий - путь до корневой от выбранной вверх
        /// </summary>
        /// <param name="categoryId">код категории</param>
        /// <returns></returns>
        public IEnumerable<Category> PathToRoot(int categoryId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_PathToRoot]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = categoryId;
                        connect.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<Category>();
                            while (reader.Read()) { list.Add(reader.ToCategory()); }
                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PathToRoot method failed.", ex);
            }
        }

        /// <summary>
        /// Список ближайших подкатегорий
        /// </summary>
        /// <param name="categoryId">код категории</param>
        /// <returns></returns>
        public IEnumerable<Category> GetSubCategories(int categoryId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_SubCategories]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = categoryId;
                        connect.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<Category>();
                            while (reader.Read()) { list.Add(reader.ToCategory()); }
                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetSubCategories method failed.", ex);
            }
        }

        /// <summary>
        /// список всех подкатегорий выбранной, со всех уровней вложенности до самого дна дерева
        /// </summary>
        /// <param name="categoryId">код категории</param>
        /// <returns></returns>
        public IEnumerable<Category> GetAllSubCategories(int categoryId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Category_AllSubCategories]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = categoryId;
                        connect.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<Category>();
                            while (reader.Read()) { list.Add(reader.ToCategory()); }
                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllSubCategories method failed.", ex);
            }
        }

        /// <summary>
        /// Кол-во моделей в выбранной категории с учетов всех уровней вложенности по дереву до самого низа
        /// </summary>
        /// <param name="categoryId">код категории</param>
        /// <returns></returns>
        public int CountModels(int categoryId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connect;
                        cmd.CommandText = "SELECT [dbo].[GetModelsCount] (@categoryId)";
                        cmd.Parameters.Add(new SqlParameter("@categoryID", SqlDbType.Int)).Value = categoryId;
                        connect.Open();
                        //используем тип SqlCommand для возвращения одного значения из БД как result set
                        var result = cmd.ExecuteScalar();
                        if (result is Int32) { return (int)result; }
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CountModels method failed.", ex);
            }
        }

        public void Dispose()
        {
            //здесь освобождаются неуправляемые ресурсы, если они есть в объекте:
            // - открытые файлы
            // - соедения с БД
            // - сокеты и т.д.
        }
    }
}
