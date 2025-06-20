using System.ComponentModel;

namespace MinimalApi.TodoList.Enums
{
    public enum CriticalityEnum
    {
        [Description("Low importance – not time-sensitive.")]
        Low = 1,

        [Description("Moderate importance – should be addressed soon.")]
        Medium = 2,

        [Description("High importance – time-sensitive.")]
        High = 3,

        [Description("Critical task – urgent and must be addressed immediately.")]
        Critical = 4,

        [Description("Blocker – prevents progress and requires top priority.")]
        Blocker = 5
    }
}
