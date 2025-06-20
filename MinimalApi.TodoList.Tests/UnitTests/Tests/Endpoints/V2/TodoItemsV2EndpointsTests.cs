using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.TodoList.DTOs.V2;
using MinimalApi.TodoList.Endpoints;
using MinimalApi.TodoList.Enums;
using MinimalApi.TodoList.Models;
using MinimalApi.TodoList.Tests.UnitTests.Base;
using MinimalApi.TodoList.Tests.UnitTests.Base.Mock;

namespace MinimalApi.TodoList.Tests.UnitTests.Tests.Endpoints.V2
{
    public class TodoItemsV2EndpointsTests
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
            var result = await TodoItemsEndpoint.GetTodoByIdV2(1, todoContext, context);

            // Assert
            var okResult = Assert.IsType<Ok<TodoItemV2Dto>>(result);
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

            var result = await TodoItemsEndpoint.GetAllTodosV2(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemV2Dto>>>(result);
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

            var result = await TodoItemsEndpoint.GetAllCompletedV2(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemV2Dto>>>(result);
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

            var result = await TodoItemsEndpoint.GetAllNotCompletedV2(db, context);
            var okResult = Assert.IsType<Ok<List<TodoItemV2Dto>>>(result);
            Assert.Single(okResult.Value);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedResult()
        {
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();
            var context = TestBase.GenerateAuthHttpContext(userId);
            var date = DateTime.Now;

            var dto = new CreateTodoItemV2Dto("New Task", date, Enums.CriticalityEnum.Low);

            var result = await TodoItemsEndpoint.CreateTodoV2(dto, db, context);

            var created = Assert.IsType<Created<TodoItemV2Dto>>(result);
            Assert.Equal("New Task", created.Value.Name);
            Assert.Equal(date, created.Value.Deadline);
            Assert.Equal(CriticalityEnum.Low, created.Value.Criticality);
        }

        [Theory]
        [InlineData(CriticalityEnum.Blocker, false, typeof(BadRequest<string>))] // Criticality 5 and Incomplete  
        [InlineData(CriticalityEnum.Blocker, true, typeof(NoContent))]  // Criticality 5 and Complete  
        [InlineData(CriticalityEnum.Low, false, typeof(NoContent))] // Other Criticality  
        public async Task DeleteTodo_HandlesCriticalityAndCompletionProperly(
                   CriticalityEnum criticality, bool isComplete, Type expectedResultType)
        {
            // Arrange  
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Id = 1, Name = "Todo Item", UserId = userId, Deadline = DateTime.Now, Criticality = criticality, IsComplete = isComplete });

            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            // Act  
            var result = await TodoItemsEndpoint.DeleteTodoV2(1, db, context);

            // Assert  
            Assert.IsType(expectedResultType, result);
        }

        [Fact]
        public async Task ChangeCriticalityV2_UpdatesCriticalitySuccessfully()
        {
            // Arrange
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Id = 1, Name = "Task", UserId = userId, Criticality = CriticalityEnum.Low });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);
            var newCriticalityDto = new ChangeCriticalityV2Dto( CriticalityEnum.High);

            // Act
            var result = await TodoItemsEndpoint.ChangeCriticalityV2(1, newCriticalityDto, db, context);

            // Assert
            var noContentResult = Assert.IsType<NoContent>(result);
            var updatedTodo = await db.Todos.FindAsync(1);
            Assert.NotNull(updatedTodo);
            Assert.Equal(newCriticalityDto.Criticality, updatedTodo.Criticality);
        }

        [Fact]
        public async Task ChangeDeadlineV2_UpdatesDeadlineSuccessfully()
        {
            // Arrange
            await using var db = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            db.Todos.Add(new TodoItem { Id = 1, Name = "Task", UserId = userId, Deadline = DateTime.Now });
            await db.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);
            var newDeadlineDto = new ChangeDeadlineV2Dto(DateTime.Now.AddDays(7));

            // Act
            var result = await TodoItemsEndpoint.ChangeDeadlineV2(1, newDeadlineDto, db, context);

            // Assert
            var noContentResult = Assert.IsType<NoContent>(result);
            var updatedTodo = await db.Todos.FindAsync(1);
            Assert.NotNull(updatedTodo);
            Assert.Equal(newDeadlineDto.NewDeadline, updatedTodo.Deadline);
        }


    }
}
