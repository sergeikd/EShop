﻿using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace EShop.Entity
{
    public class Category
    {
        public int? ParentId { get; set; }
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string Name { get; set; }

        //navigation propertiy
        public virtual ICollection<Model> Models { get; set; }

        public Category()
        {
            ParentId = 0;
            Id = 0;
            ImageId = 0;
            Name = string.Empty;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} {2}", Id, ParentId, Name);
        }
    }

    public class Delivery
    {
        public short Id { get; set; }
        public string DeliveryType { get; set; }

        //navigation property
        public virtual ICollection<Model> Models { get; set; }
        //static метод генерации дефолтного объекта Delivery
        public static Delivery Default
        {
            get { return new Delivery() { Id = 2, DeliveryType = "Бесплатная" }; }
        }
    }

    public class Availability
    {
        public short Id { get; set; }
        public string AvailabilityType { get; set; }

        //navigation property
        public virtual ICollection<Model> Models { get; set; }
        //static метод генерации дефолтного объекта Availability
        public static Availability Default
        {
            get { return new Availability() { Id = 2, AvailabilityType = "Склад" }; }
        }
    }

    public class Model
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
        public short Warranty { get; set; }

        //Foreign key's
        public int CategoryId { get; set; }
        public short AvailabilityId { get; set; }
        public short DeliveryId { get; set; }
        public int Comments { get; set; }

        //navigation property
        public virtual Category Category { get; set; }
        public virtual Availability Availability { get; set; }
        public virtual Delivery Delivery { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.({1}) {2} {3}", Id,
                        Category != null ? Category.Name : "Not defined",
                        Price.ToString("0,0.0", CultureInfo.GetCultureInfo("en-US")),
                        Title);
        }
    }

    //Сущность для регистрации нового пользователя
    public class NewUser
    {
        public int Id { get; set; }
        public string RegKey { get; set; }
        public string RegTime { get; set; }
    }

    public class Comments
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public string CommentTime { get; set; }
    }

    //Сущности категорий для построения полного дерева
    public class Layer3
    {
        public int CategoryId3 { get; set; }
        public string CategoryName3 { get; set; }
    }

    public class Layer2
    {
        public int CategoryId2 { get; set; }
        public string CategoryName2 { get; set; }
        public List<Layer3> Categories2 { get; set; }
    }

    public class Layer1
    {
        public int CategoryId1 { get; set; }
        public string CategoryName1 { get; set; }
        public List<Layer2> Categories1 { get; set; }
    }
}
