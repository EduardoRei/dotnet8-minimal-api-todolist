using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options) { }

        public DbSet<TodoItem> Todos => Set<TodoItem>();
    }
}
