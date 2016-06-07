using EShop.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace EshopAdoDataProvider
{
    public class AdoCommentsRepository : ICommentsRepository
    {
        private readonly string _connectionString;
        public AdoCommentsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Comments> GetComments(int productId)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Comments_Get]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int)).Value = productId;
                        connect.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<Comments>();
                            while (reader.Read())
                            {
                                list.Add(reader.ToComments());
                            }
                            return list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetComments method failed.", ex);
            }
        }

        public bool SaveComment(Comments newComment)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Comment_Add]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int, 512)).Value = newComment.UserId;
                        cmd.Parameters.Add(new SqlParameter("@UserName", SqlDbType.VarChar, 256)).Value = newComment.UserName;
                        cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int, 512)).Value = newComment.ProductId;
                        cmd.Parameters.Add(new SqlParameter("@Comment", SqlDbType.VarChar, 1024)).Value = newComment.Comment;
                        cmd.Parameters.Add(new SqlParameter("@DateTime", SqlDbType.VarChar, 32)).Value = newComment.CommentTime;
                        connect.Open();
                        var result = cmd.ExecuteNonQuery();
                        //var value = cmd.Parameters["@return_value"].Value;
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SaveComment method failed.", ex);
            }
        }
        public bool UpdateComment(Comments comment)
        {
            try
            {
                using (var connect = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("[Comment_Update]", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int, 512)).Value = comment.UserId;
                        cmd.Parameters.Add(new SqlParameter("@UserName", SqlDbType.VarChar, 256)).Value = comment.UserName;
                        cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int, 512)).Value = comment.ProductId;
                        cmd.Parameters.Add(new SqlParameter("@Comment", SqlDbType.VarChar, 1024)).Value = comment.Comment;
                        cmd.Parameters.Add(new SqlParameter("@DateTime", SqlDbType.VarChar, 32)).Value = comment.CommentTime;

                        connect.Open();
                        var result = cmd.ExecuteNonQuery();

                        return (result > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Comment_Update method failed.", ex);
            }
        }
        public void Dispose()
        {
        }


    }
}
