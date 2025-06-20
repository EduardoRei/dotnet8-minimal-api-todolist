using Asp.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;
using MinimalApi.TodoList.DTOs.Shared;
using MinimalApi.TodoList.DTOs.V1;
using MinimalApi.TodoList.DTOs.V2;
using MinimalApi.TodoList.Extensions;
using MinimalApi.TodoList.Models;
using System.Security.Claims;

namespace MinimalApi.TodoList.Endpoints
{
    public static class TodoItemsEndpoint
    {

        public static void MapTodoItemsEndpoints(this IEndpointRouteBuilder app)
        {
            MapSharedEndpoints(app);

            MapV1(app);

            MapV2(app);
        }

        private static void MapSharedEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPut("/todoitems/{id}" + "/ChangeName", ChangeNameTodo)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);

            app.MapPut("/todoitems/{id}" + "/SetCompleted", SetCompletedTodo)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);
        }

        private static void MapV1(this IEndpointRouteBuilder app)
        {
            app.MapGet("/todoitems", GetAllTodosV1)
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK, typeof(IEnumerable<TodoItemV1Dto>));

            app.MapGet("/todoitems/{id}", GetTodoByIdV1)
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status200OK, typeof(TodoItemV1Dto));

            app.MapDelete("/todoitems/{id}", DeleteTodoV1)
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);

            app.MapPost("/todoitems", CreateTodoV1)
                .MapToApiVersion(1, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status201Created, typeof(TodoItemV1Dto));
        }

        private static void MapV2(this IEndpointRouteBuilder app)
        {
            app.MapGet("/todoitems", GetAllTodosV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK, typeof(IEnumerable<TodoItemV2Dto>)); ;

            app.MapGet("/todoitems/{id}", GetTodoByIdV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status200OK, typeof(TodoItemV2Dto));

            app.MapDelete("/todoitems/{id}", DeleteTodoV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);

            app.MapPost("/todoitems", CreateTodoV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status201Created, typeof(TodoItemV2Dto));

            app.MapGet("/todoitems/AllCompleted", GetAllCompletedV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK, typeof(IEnumerable<TodoItemV2Dto>));

            app.MapGet("/todoitems/AllNotCompleted", GetAllNotCompletedV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK, typeof(IEnumerable<TodoItemV2Dto>));

            app.MapPut("/todoitems/{id}" + "/ChangeDeadline", ChangeDeadlineV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);


            app.MapPut("/todoitems/{id}" + "/ChangeCriticality", ChangeCriticalityV2)
                .MapToApiVersion(2, 0)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status204NoContent);
        }

        #region Shared
        public static async Task<IResult> SetCompletedTodo(int id, SetCompletedTodoItemDto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return TypedResults.NotFound();

            if (todoItem.IsComplete == todo.IsComplete)
                return TypedResults.NoContent();

            todoItem.IsComplete = todo.IsComplete;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public static async Task<IResult> ChangeNameTodo(int id, ChangeNameTodoItemDto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return Results.NotFound();

            todoItem.Name = todo.Name;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        #endregion

        #region V1
        public static async Task<IResult> GetAllTodosV1(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var todos = await db.Todos.Where(t => t.UserId == userId)
                .Select(t => t.ToDtoV1())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetTodoByIdV1(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem is null)
                return Results.NotFound();

            return Results.Ok(todoItem.ToDtoV1());
        }

        public static async Task<IResult> DeleteTodoV1(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem is null)
                return TypedResults.NotFound();

            db.Todos.Remove(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public static async Task<IResult> CreateTodoV1(CreateTodoItemV1Dto dto, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = new TodoItem
            {
                Name = dto.Name,
                IsComplete = false,
                UserId = userId
            };

            db.Todos.Add(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/todoitems", todoItem.ToDtoV1());
        }
        #endregion

        #region V2
        public static async Task<IResult> GetAllTodosV2(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var todos = await db.Todos.Where(t => t.UserId == userId)
                .Select(t => t.ToDtoV2())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetAllNotCompletedV2(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var todos = await db.Todos
                .Where(t => !t.IsComplete && t.UserId == userId)
                .Select(t => t.ToDtoV2())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetAllCompletedV2(TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todos = await db.Todos
                .Where(t => t.IsComplete && t.UserId == userId)
                .Select(t => t.ToDtoV2())
                .ToListAsync();

            return Results.Ok(todos);
        }

        public static async Task<IResult> GetTodoByIdV2(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem != null)
                return Results.Ok(todoItem.ToDtoV2());

            return Results.NotFound();
        }

        public static async Task<IResult> DeleteTodoV2(int id, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todoItem is null)
                return Results.NotFound();

            if(todoItem.Criticality is Enums.CriticalityEnum.Blocker
                && todoItem.IsComplete is false)
                return Results.BadRequest("Cannot delete a blocker task that is not completed.");

            db.Todos.Remove(todoItem);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        public static async Task<IResult> CreateTodoV2(CreateTodoItemV2Dto dto, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = new TodoItem
            {
                Name = dto.Name,
                IsComplete = false,
                UserId = userId,
                Deadline = dto.Deadline,
                Criticality = dto.Criticality
            };

            db.Todos.Add(todoItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/todoitems", todoItem.ToDtoV2());
        }

        public static async Task<IResult> ChangeDeadlineV2(int id, ChangeDeadlineV2Dto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return TypedResults.NotFound();

            if (todoItem.Deadline == todo.NewDeadline)
                return TypedResults.NoContent();

            todoItem.Deadline = todo.NewDeadline;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        public static async Task<IResult> ChangeCriticalityV2(int id, ChangeCriticalityV2Dto todo, TodoDbContext db, HttpContext http)
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Results.Unauthorized();

            var todoItem = await db.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (todoItem is null)
                return TypedResults.NotFound();

            if (todoItem.Criticality == todo.Criticality)
                return TypedResults.NoContent();

            todoItem.Criticality = todo.Criticality;
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        #endregion

    }
}
