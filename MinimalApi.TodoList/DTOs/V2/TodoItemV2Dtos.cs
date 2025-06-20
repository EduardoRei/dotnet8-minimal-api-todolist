using MinimalApi.TodoList.Enums;

namespace MinimalApi.TodoList.DTOs.V2
{
    public record CreateTodoItemV2Dto(string Name, DateTime Deadline, CriticalityEnum Criticality);
    public record ChangeDeadlineV2Dto(DateTime NewDeadline);
    public record ChangeCriticalityV2Dto(CriticalityEnum Criticality);
    public record TodoItemV2Dto(int Id, string Name, bool IsComplete, DateTime CreatedAt, DateTime? Deadline, CriticalityEnum? Criticality);


}
