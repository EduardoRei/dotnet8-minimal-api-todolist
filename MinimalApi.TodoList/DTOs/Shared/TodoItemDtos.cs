namespace MinimalApi.TodoList.DTOs.Shared
{
    public record ChangeNameTodoItemDto(string Name);
    public record SetCompletedTodoItemDto(bool IsComplete);
}
