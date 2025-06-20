using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.TodoList.DTOs.V1;
using MinimalApi.TodoList.Endpoints;
using MinimalApi.TodoList.Models;
using MinimalApi.TodoList.Tests.UnitTests.Base;
using MinimalApi.TodoList.Tests.UnitTests.Base.Mock;

namespace MinimalApi.TodoList.Tests.UnitTests.Tests.Endpoints.V1
{
    public class TodoItemsV1EndpointsTests
    {
        [Fact]
        public async Task GetTodoByID_ReturnsOk_WithUserTodo()
        {
            // Arrange
            await using var todoContext = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            todoContext.Todos.Add(new TodoItem { Name = "Task 1", UserId = userId });
            await todoContext.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            // Act
            var result = await TodoItemsEndpoint.GetTodoByIdV1(1, todoContext, context);

            // Assert
            var okResult = Assert.IsType<Ok<TodoItemV1Dto>>(result);
            Assert.Equal("Task 1", okResult.Value.Name);
        }

        [Fact]
        public async Task GetAllTodos_ReturnsOk_WithUserTodos()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Name = "Task A", UserId = userId });
            db.Todos.Add(new TodoItem { Name = "Task B", UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            var result = await TodoItemsEndpoint.GetAllTodosV1(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemV1Dto>>>(result);
            Assert.Equal(2, okResult.Value.Count);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedResult()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            var context = TestBase.GenerateAuthHttpContext(userId);

            var dto = new CreateTodoItemV1Dto("New Task");

            var result = await TodoItemsEndpoint.CreateTodoV1(dto, db, context);

            var created = Assert.IsType<Created<TodoItemV1Dto>>(result);
            Assert.Equal("New Task", created.Value.Name);
        }

        [Fact]
        public async Task DeleteTodo_RemovesItem_WhenExists()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            db.Todos.Add(new TodoItem { Id = 1, Name = "To delete", UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            var result = await TodoItemsEndpoint.DeleteTodoV1(1, db, context);
            Assert.IsType<NoContent>(result);
        }

    }
}
