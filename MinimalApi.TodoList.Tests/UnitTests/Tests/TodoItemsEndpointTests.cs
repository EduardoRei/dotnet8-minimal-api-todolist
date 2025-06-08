using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.TodoList.DTOs;
using MinimalApi.TodoList.Endpoints;
using MinimalApi.TodoList.Models;
using MinimalApi.TodoList.Tests.UnitTests.Base.Mock;
using MinimalApi.TodoList.Tests.UnitTests.Base;

namespace MinimalApi.TodoList.Tests.UnitTests.Tests
{
    public class TodoItemsEndpointTests
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
            var result = await TodoItemsEndpoint.GetTodoById(1,todoContext, context);

            // Assert
            var okResult = Assert.IsType<Ok<TodoItemDto>>(result);
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

            var result = await TodoItemsEndpoint.GetAllTodos(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemDto>>>(result);
            Assert.Equal(2, okResult.Value.Count);
        }

        [Fact]
        public async Task GetAllCompleted_ReturnsOnlyCompletedTodos()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Name = "Done", IsComplete = true, UserId = userId });
            db.Todos.Add(new TodoItem { Name = "Pending", IsComplete = false, UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            var result = await TodoItemsEndpoint.GetAllCompleted(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemDto>>>(result);
            Assert.Single(okResult.Value);
        }

        [Fact]
        public async Task GetAllNotCompleted_ReturnsOnlyIncompleteTodos()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Name = "Incomplete Task", IsComplete = false, UserId = userId });
            db.Todos.Add(new TodoItem { Name = "Complete Task", IsComplete = true, UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            var result = await TodoItemsEndpoint.GetAllNotCompleted(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemDto>>>(result);
            Assert.Single(okResult.Value);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedResult()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            var context = TestBase.GenerateAuthHttpContext(userId);

            var dto = new CreateTodoItemDto ("New Task" );

            var result = await TodoItemsEndpoint.CreateTodo(dto, db, context);

            var created = Assert.IsType<Created<TodoItemDto>>(result);
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

            var result = await TodoItemsEndpoint.DeleteTodo(1, db, context);
            Assert.IsType<NoContent>(result);
        }

        [Fact]
        public async Task SetCompletedTodo_SetsAsCompleted_WhenValid()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            db.Todos.Add(new TodoItem { Id = 1, Name = "To Complete", IsComplete = false, UserId = userId });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);
            var dto = new SetCompletedTodoItemDto (true);

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
