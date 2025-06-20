namespace MinimalApi.TodoList.DTOs.V1
{
    public record CreateTodoItemV1Dto(string Name);

    public record TodoItemV1Dto(int Id, string Name, bool IsComplete, DateTime CreatedAT);


}
