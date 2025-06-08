using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;

namespace MinimalApi.TodoList.Tests.UnitTests.Base.Mock
{
    public class MockTodoDb : IDbContextFactory<TodoDbContext>
    {
        public TodoDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase($"TodoTestDb-{Guid.NewGuid()}")
                .Options;

            return new TodoDbContext(options);
        }
    }
}
