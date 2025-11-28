using Microsoft.EntityFrameworkCore;
using PizzaStore.SQLite.API.Models;

namespace PizzaStore.SQLite.API.Data
{
    public class PizzaDb : DbContext
    {
        public PizzaDb(DbContextOptions<PizzaDb> options) : base(options) { }
        public DbSet<Pizza> Pizzas => Set<Pizza>();
    }
}
