using Microsoft.EntityFrameworkCore;
 
namespace TodoApp.Models
{
    public class TodoContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<User> users {get;set;}
        public DbSet<Todo> todos {get;set;}
    }
}