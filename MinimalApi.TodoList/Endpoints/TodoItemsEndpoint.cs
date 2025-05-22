using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;
using MinimalApi.TodoList.DTOs;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Endpoints
{
    public static class TodoItemsEndpoint
    {
        public static void MapTodoItemsEndpoints(this WebApplication app)
        {
            app.MapGet("/todoitems", async (TodoDbContext db) =>
            {
                return await db.Todos.ToListAsync();
            });

            app.MapGet("/todoitems/{id}", async (int id, TodoDbContext db) =>
            {
                return await db.Todos.FindAsync(id)
                    is TodoItem todoItem
                        ? Results.Ok(todoItem)
                        : Results.NotFound();
            });

            app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null) 
                    return Results.NotFound();
                
                db.Todos.Remove(todoItem);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapPost("/todoitems", async (CreateTodoItemDto createTodo, TodoDbContext db) =>
            {
                var todoItem = new TodoItem
                {
                    Name = createTodo.Name,
                    IsComplete = false
                };

                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();
                return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
            });

            app.MapPut("/todoitems/{id}/ChangeName", async (int id, ChangeNameTodoItemDto updateTodo, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null)
                    return Results.NotFound();
                todoItem.Name = updateTodo.Name;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapPut("/todoitems/{id}/SetCompleted", async (int id, SetCompletedTodoItemDto updateTodo, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null)
                    return Results.NotFound();

                if(todoItem.IsComplete == updateTodo.IsComplete)
                    return Results.NoContent();

                todoItem.IsComplete = updateTodo.IsComplete;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
