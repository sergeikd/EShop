using System.Data.Entity;

namespace EShop.MVC2.Models
{
    public class UserEntityContext : DbContext
    {
        public UserEntityContext() : base("eshop.db") { }
        public DbSet<UserModel> UserEntities { get; set; }
    }
}