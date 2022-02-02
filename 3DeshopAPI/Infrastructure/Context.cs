using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        //public List<User> usersTemp = new List<User>() {
        //    new User { Id = Guid.NewGuid(), Username = "user1", FirstName = "nojus", LastName = "Rg", Email = "n@gmai.c", Password = "123" },
        //    new User { Id = Guid.NewGuid(), Username = "u21", FirstName = "ki", LastName = "kve", Email = "a@gmai.c", Password = "88448" }
        //};

        public Context(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<User> Users { get; set; }
    }
}