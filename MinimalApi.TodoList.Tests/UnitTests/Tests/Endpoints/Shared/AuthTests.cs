﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApi.TodoList.DTOs.V1;
using MinimalApi.TodoList.Endpoints;
using MinimalApi.TodoList.Models;
using MinimalApi.TodoList.Tests.UnitTests.Base;
using MinimalApi.TodoList.Tests.UnitTests.Base.Mock;
using System.Security.Claims;

namespace MinimalApi.TodoList.Tests.UnitTests.Tests.Endpoints.Shared
{
    public class AuthTests
    {

        [Fact]
        public async Task GetAllTodos_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            // Arrange
            await using var todoContext = new MockTodoDb().CreateDbContext();
            var context = new DefaultHttpContext { User = new ClaimsPrincipal() }; // sem identidade

            // Act
            var result = await TodoItemsEndpoint.GetAllTodosV1(todoContext, context);

            // Assert
            Assert.IsType<UnauthorizedHttpResult>(result);
        }

        [Fact]
        public async Task GetAllTodos_ReturnsOk_WithUserTodos()
        {
            // Arrange
            await using var todoContext = new MockTodoDb().CreateDbContext();
            var userId = Guid.NewGuid().ToString();

            todoContext.Todos.Add(new TodoItem { Name = "Task 1", UserId = userId });
            await todoContext.SaveChangesAsync();

            var context = TestBase.GenerateAuthHttpContext(userId);

            // Act
            var result = await TodoItemsEndpoint.GetAllTodosV1(todoContext, context);

            // Assert
            var okResult = Assert.IsType<Ok<List<TodoItemV1Dto>>>(result);
            Assert.Single(okResult.Value);
            Assert.Equal("Task 1", okResult.Value[0].Name);
        }
    }
}
