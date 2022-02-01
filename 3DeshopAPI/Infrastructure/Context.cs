using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context /*: DbContext*/
    {
        public List<User> users = new List<User>() {
            new User { Id = Guid.NewGuid(), Username = "user1", FirstName = "nojus", LastName = "Rg", Email = "n@gmai.c", Password = "123" },
            new User { Id = Guid.NewGuid(), Username = "u21", FirstName = "ki", LastName = "Rg", Email = "a@gmai.c", Password = "88448" }};
        //public Context(DbContextOptions dbContextOptions) : base(dbContextOptions)
        //{
        //    users = new DbSet<User>()
        //    { new User { Id = Guid.NewGuid(), Username = "user1", FirstName = "nojus", LastName = "Rg", Email = "n@gmai.c", Password = "123" };
        //}

        //public DbSet<User> users { get; set; }
    }
}