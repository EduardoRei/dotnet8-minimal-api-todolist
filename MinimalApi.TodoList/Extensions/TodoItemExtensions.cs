using MinimalApi.TodoList.DTOs.V1;
using MinimalApi.TodoList.DTOs.V2;
using MinimalApi.TodoList.Models;

namespace MinimalApi.TodoList.Extensions
{
    public static class TodoItemExtensions
    {
        public static TodoItemV1Dto ToDtoV1(this TodoItem item) =>
            new(item.Id, item.Name, item.IsComplete, item.CreatedAt);

        public static TodoItemV2Dto ToDtoV2(this TodoItem item) =>
            new(item.Id, item.Name, item.IsComplete, item.CreatedAt, item.Deadline, item.Criticality);
    }
}
