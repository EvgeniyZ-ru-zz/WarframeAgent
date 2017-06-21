using System.Data.Entity;
using Core.Model;

namespace Core
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }
        public DbSet<Item> Items { get; set; }
    }
}
