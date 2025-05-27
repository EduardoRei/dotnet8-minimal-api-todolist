using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;
using MinimalApi.TodoList.DTOs;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Endpoints
{
    public static class TodoItemsEndpoint
    {
        private const string BaseRoute = "/api/v{version:apiVersion}/todoitems";
        private const string ByIdRoute = BaseRoute + "/{id}";
        
        public static void MapTodoItemsEndpoints(this WebApplication app)
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(1, 0)
                .HasApiVersion(2, 0)
                .ReportApiVersions()
                .Build();

            app.MapGet(BaseRoute, async (TodoDbContext db) =>
            {
                return await db.Todos.ToListAsync();
            })
            .WithApiVersionSet(versionSet);

            app.MapGet(ByIdRoute, async (int id, TodoDbContext db) =>
            {
                return await db.Todos.FindAsync(id)
                    is TodoItem todoItem
                        ? Results.Ok(todoItem)
                        : Results.NotFound();
            })
            .WithApiVersionSet(versionSet);

            app.MapDelete(ByIdRoute, async (int id, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null) 
                    return Results.NotFound();
                
                db.Todos.Remove(todoItem);
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithApiVersionSet(versionSet);

            app.MapPost(BaseRoute, async (CreateTodoItemDto createTodo, TodoDbContext db) =>
            {
                var todoItem = new TodoItem
                {
                    Name = createTodo.Name,
                    IsComplete = false
                };

                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();
                return Results.Created($"{BaseRoute}/{todoItem.Id}", todoItem);
            })
            .WithApiVersionSet(versionSet);

            app.MapPut(ByIdRoute + "/ChangeName", async (int id, ChangeNameTodoItemDto updateTodo, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null)
                    return Results.NotFound();
                todoItem.Name = updateTodo.Name;
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithApiVersionSet(versionSet);

            app.MapPut(ByIdRoute + "/SetCompleted", async (int id, SetCompletedTodoItemDto updateTodo, TodoDbContext db) =>
            {
                var todoItem = await db.Todos.FindAsync(id);
                if (todoItem is null)
                    return Results.NotFound();

                if(todoItem.IsComplete == updateTodo.IsComplete)
                    return Results.NoContent();

                todoItem.IsComplete = updateTodo.IsComplete;
                await db.SaveChangesAsync();
                return Results.NoContent();
            })
            .WithApiVersionSet(versionSet);

            app.MapGet(BaseRoute + "/AllCompleted", async (TodoDbContext db) =>
            {
                return await db.Todos
                    .Where(t => t.IsComplete)
                    .ToListAsync();
            })
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(2, 0);

            app.MapGet(BaseRoute + "/AllNotCompleted", async (TodoDbContext db) =>
            {
                return await db.Todos
                    .Where(t => !t.IsComplete)
                    .ToListAsync();
            })
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(2, 0);
        }
    }
}
