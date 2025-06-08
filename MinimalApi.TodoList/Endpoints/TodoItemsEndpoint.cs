using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;
using MinimalApi.TodoList.DTOs;
using MinimalApi.TodoList.Extensions;
using MinimalApi.TodoList.Models;
using System.Security.Claims;

namespace MinimalApi.TodoList.Endpoints
{
    public static class TodoItemsEndpoint
    {      

        public static void MapTodoItemsEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/todoitems", GetAllTodos);

            app.MapGet("/todoitems/{id}", GetTodoById);

            app.MapDelete("/todoitems/{id}", DeleteTodo);

            app.MapPost("/todoitems", CreateTodo);

            app.MapPut("/todoitems/{id}" + "/ChangeName", ChangeNameTodo);

            app.MapPut("/todoitems/{id}" + "/SetCompleted", SetCompletedTodo);

            app.MapGet("/todoitems/AllCompleted", GetAllCompleted)
            .MapToApiVersion(2, 0);

            app.MapGet("/todoitems/AllNotCompleted", GetAllNotCompleted)
            .MapToApiVersion(2, 0);
        }

        public static async Task<IResult> GetAllTodos(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Results.Unauthorized();

            var todos = await db.Todos.Where(t => t.UserId == userId)
                .Select(t => t.ToDto())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetAllNotCompleted(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Results.Unauthorized();

            var todos = await db.Todos
                .Where(t => !t.IsComplete && t.UserId == userId)
                .Select(t => t.ToDto())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetAllCompleted(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Results.Unauthorized();

            var todos = await db.Todos
                .Where(t => t.IsComplete && t.UserId == userId)
                .Select(t => t.ToDto())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetTodoById(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if(todoItem != null)
                return Results.Ok(todoItem.ToDto());

            return Results.NotFound();
        }

        public static async Task<IResult> DeleteTodo(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem is null)
                return TypedResults.NotFound();

            db.Todos.Remove(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public static async Task<Created<TodoItemDto>> CreateTodo(CreateTodoItemDto dto, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todoItem = new TodoItem
            {
                Name = dto.Name,
                IsComplete = false,
                UserId = userId
            };

            db.Todos.Add(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/todoitems", todoItem.ToDto());
        }

        public static async Task<IResult> SetCompletedTodo(int id, SetCompletedTodoItemDto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return TypedResults.NotFound();

            if (todoItem.IsComplete == todoItem.IsComplete)
                return TypedResults.NoContent();

            todoItem.IsComplete = todoItem.IsComplete;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public static async Task<IResult> ChangeNameTodo(int id, ChangeNameTodoItemDto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return TypedResults.NotFound();

            todoItem.Name = todo.Name;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    }
}
