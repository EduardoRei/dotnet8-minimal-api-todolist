using Microsoft.EntityFrameworkCore;
using MinimalApi.TodoList.Data;
using MinimalApi.TodoList.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Implementing Swagger and redirecting at startup
app.UseSwagger();
app.UseSwaggerUI(); 

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

//Adding Endpoint
app.MapTodoItemsEndpoints();

app.Run();
