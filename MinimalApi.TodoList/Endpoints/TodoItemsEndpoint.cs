using Asp.Versioning.Conventions;
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

            app.MapGet("/todoitems", async (TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return await db.Todos.Where(t => t.UserId == userId)
                .Select(t => t.ToDto())
                .ToListAsync();
            });

            app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                return todoItem is null ? Results.NotFound() : Results.Ok(todoItem.ToDto());
            });

            app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (todoItem is null)
                    return Results.NotFound();

                db.Todos.Remove(todoItem);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapPost("/todoitems", async (CreateTodoItemDto dto, TodoDbContext db, HttpContext http) =>
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
                return Results.Created($"/todoitems", todoItem.ToDto());
            });

            app.MapPut("/todoitems/{id}" + "/ChangeName", async (int id, ChangeNameTodoItemDto updateTodo, TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
                if (todoItem is null)
                    return Results.NotFound();
                todoItem.Name = updateTodo.Name;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapPut("/todoitems/{id}" + "/SetCompleted", async (int id, SetCompletedTodoItemDto updateTodo, TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
                if (todoItem is null)
                    return Results.NotFound();

                if(todoItem.IsComplete == updateTodo.IsComplete)
                    return Results.NoContent();

                todoItem.IsComplete = updateTodo.IsComplete;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapGet("/todoitems/AllCompleted", async (TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

                return await db.Todos
                    .Where(t => t.IsComplete && t.UserId == userId)
                    .Select(t => t.ToDto())
                    .ToListAsync();
            })
            .MapToApiVersion(2, 0);

            app.MapGet("/todoitems/AllNotCompleted", async (TodoDbContext db, HttpContext http) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

                return await db.Todos
                    .Where(t => !t.IsComplete && t.UserId == userId)
                    .Select(t => t.ToDto())
                    .ToListAsync();
            })
            .MapToApiVersion(2, 0);
        }
    }
}
