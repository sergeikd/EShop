using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EShop.Entity;
using System.Data.SqlClient;

namespace EshopAdoDataProvider
{
    static class DataReaderEx
    {
        public static Category ToCategory(this SqlDataReader reader)
        {
            return new Category()
            {
                Id = reader["ID"] != DBNull.Value ? (int)reader["ID"] : 0,
                ImageId = reader["ImageID"] != DBNull.Value ? (int)reader["ImageID"] : 0,
                ParentId = reader["ParentId"] != DBNull.Value ? (int)reader["ParentId"] : 0,
                Name = reader["Name"].ToString()
            };
        }

        public static Model ToModel(this SqlDataReader reader)
        {
            //return new Model()
            //{
            //    Id = reader["ID"] != DBNull.Value ? (int)reader["ID"] : 0,
            //    CategoryId = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : 0,
            //    ImageId = reader["ImageId"] != DBNull.Value ? (int)reader["ImageId"] : (int?)null,
            //    Title = reader["Title"].ToString(),
            //    Description = reader["Description"].ToString(),
            //    Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
            //    AvailabilityId = (short)(reader["Availability"] != DBNull.Value ? (short)reader["Availability"] : 0),
            //    DeliveryId = (short)(reader["Delivery"] != DBNull.Value ? (short)reader["Delivery"] : 0),
            //    Warranty = (short)(reader["Warranty"] != DBNull.Value ? (short)reader["Warranty"] : 0)
            //};       
            Model model = new Model()
                {
                    Id = reader["ID"] != DBNull.Value ? (int)reader["ID"] : 0,
                    CategoryId = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : 0,
                    ImageId = reader["ImageId"] != DBNull.Value ? (int)reader["ImageId"] : (int?)null,
                    Title = reader["Title"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
                    AvailabilityId = (short)(reader["Availability"] != DBNull.Value ? (short)reader["Availability"] : 0),
                    DeliveryId = (short)(reader["Delivery"] != DBNull.Value ? (short)reader["Delivery"] : 0),
                    Warranty = (short)(reader["Warranty"] != DBNull.Value ? (short)reader["Warranty"] : 0)
                };

            //добиваем модель недостававшими данными (этого не было в материалах курса, хранимую процедуру в SQL сервере тоже пришлось обновить)
            Delivery delivery = new Delivery() 
                {  
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Id = model.DeliveryId
                };
            model.Delivery = delivery;

            Availability availability = new Availability()
                {
                    AvailabilityType = reader["AvailabilityType"].ToString(),
                    Id = model.AvailabilityId
                };         
            model.Availability = availability;

            Category category = new Category() { Name = reader["Name"].ToString() };
            model.Category = category;
            return model;
        }

        //Этот метод  отличается от ToModel отсутствием данных для сущностей Delivery и Availability
        //они просто не используются для отображения в списке товаров 
        //поэтому экономится время при обращении к БД
        public static Model ToModels(this SqlDataReader reader)
        {
            Model model = new Model()
            {
                Id = reader["ID"] != DBNull.Value ? (int)reader["ID"] : 0,
                CategoryId = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : 0,
                ImageId = reader["ImageId"] != DBNull.Value ? (int)reader["ImageId"] : (int?)null,
                Title = reader["Title"].ToString(),
                Description = reader["Description"].ToString(),
                Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
                AvailabilityId = (short)(reader["Availability"] != DBNull.Value ? (short)reader["Availability"] : 0),
                DeliveryId = (short)(reader["Delivery"] != DBNull.Value ? (short)reader["Delivery"] : 0),
                Warranty = (short)(reader["Warranty"] != DBNull.Value ? (short)reader["Warranty"] : 0)
            };
            return model;
        }
        public static Delivery ToDeliveryType(this SqlDataReader reader)
        {
            return new Delivery()
            {
                Id = (short)(reader["ID"] != DBNull.Value ? (short)reader["ID"] : 0),
                DeliveryType = reader["DeliveryType"].ToString()
            };
        }

        public static Availability ToAvailabilityTypes (this SqlDataReader reader)
        {
            return new Availability()
            {
                Id = (short)(reader["ID"] != DBNull.Value ? (short)reader["ID"] : 0),
                AvailabilityType = reader["AvailabilityType"].ToString()
            };
        }

        public static Delivery ToDeliveryTypes(this SqlDataReader reader)
        {
            return new Delivery()
            {
                Id = (short)(reader["ID"] != DBNull.Value ? (short)reader["ID"] : 0),
                DeliveryType = reader["DeliveryType"].ToString()
            };
        }
        public static NewUser ToNewUser(this SqlDataReader reader)
        {
            return new NewUser()
            {
                Id = (int)(reader["UserId"] != DBNull.Value ? (int)reader["UserId"] : 0),
                RegKey = reader["RegKey"].ToString(),
                RegTime = reader["RegTime"].ToString(),
            };
        }

        public static Comments ToComments(this SqlDataReader reader)
        {
            return new Comments()
            {
                UserId = (int)(reader["UserId"] != DBNull.Value ? (int)reader["UserId"] : 0),
                UserName = reader["UserName"].ToString(),
                ProductId = (int)(reader["ProductId"] != DBNull.Value ? (int)reader["ProductId"] : 0),
                Comment = reader["Comment"].ToString(),
                CommentTime = reader["DateTime"].ToString()
            };
        }
    }
}
