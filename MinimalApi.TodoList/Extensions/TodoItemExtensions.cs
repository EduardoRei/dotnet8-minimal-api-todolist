using MinimalApi.TodoList.DTOs;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Extensions
{
    public static class TodoItemExtensions
    {
        public static TodoItemDto ToDto(this TodoItem item) =>
            new(item.Id, item.Name, item.IsComplete);
    }
}
