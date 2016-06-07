using EShop.Entity;
using System;
using System.Data;
using System.Data.SqlClient;

namespace EshopAdoDataProvider
{
    public class AdoAccountRepository : IAccountRepository
    {
        private readonly string _connectionString;
        public AdoAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public NewUser GetKey(int userId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[NewUser_Get]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = userId;
                        connect.Open();
                        using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            //reader.ToCategory() - вызов расширяющего метода для класса SqlDataReader
                            //сам метод см. в классе DataReaderEx этого проекта
                            return reader.Read() ? reader.ToNewUser() : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetCategory method failed.", ex);
            }
        }

        public bool StoreKey(NewUser newUser)
        {

            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[NewUser_Add]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int, 512)).Value = newUser.Id;
                        cmd.Parameters.Add(new SqlParameter("@RegKey", SqlDbType.VarChar, 16)).Value = newUser.RegKey;
                        cmd.Parameters.Add(new SqlParameter("@RegTime", SqlDbType.VarChar, 32)).Value = newUser.RegTime;
                        connect.Open();
                        var result = cmd.ExecuteNonQuery();
                        //var value = cmd.Parameters["@return_value"].Value;
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("StoreKey method failed.", ex);
            }
        }

        //Метод для удаления записи из БД Registration
        public void DeleteRecord (int userId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[NewUser_Delete]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)).Value = userId;
                        connect.Open();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NewUser_Delete method failed.", ex);
            }
        }

        public void Dispose()
        {
        }
    }
}
