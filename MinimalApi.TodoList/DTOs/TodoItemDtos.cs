namespace MinimalApi.TodoList.DTOs
{
    public record ChangeNameTodoItemDto(string Name);
    public record CreateTodoItemDto(string Name);
    public record SetCompletedTodoItemDto(bool IsComplete);
    public record TodoItemDto(int Id, string Name, bool IsComplete);
}
