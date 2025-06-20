using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.TodoList.DTOs.Shared;
using MinimalApi.TodoList.Endpoints;
using MinimalApi.TodoList.Models;
using MinimalApi.TodoList.Tests.UnitTests.Base;
using MinimalApi.TodoList.Tests.UnitTests.Base.Mock;

namespace MinimalApi.TodoList.Tests.UnitTests.Tests
{
    public class TodoItemsSharedEndpointsTests
    {

        [Fact]
        public async Task SetCompletedTodo_SetsAsCompleted_WhenValid()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            db.Todos.Add(new TodoItem { Id = 1, Name = "To Complete", IsComplete = false, UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);
            var dto = new SetCompletedTodoItemDto(true);

            var result = await TodoItemsEndpoint.SetCompletedTodo(1, dto, db, context);
            Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task ChangeNameTodo_ChangesName_WhenValid()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            db.Todos.Add(new TodoItem { Id = 1, Name = "Old Name", UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);
            var dto = new ChangeNameTodoItemDto ( "New Name" );

            var result = await TodoItemsEndpoint.ChangeNameTodo(1, dto, db, context);
            Assert.IsType<NoContent>(result);
        }
    }
}
