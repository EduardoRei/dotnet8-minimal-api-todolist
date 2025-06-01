using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options) { }

        public DbSet<TodoItem> Todos => Set<TodoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId);
        }
    }
}
