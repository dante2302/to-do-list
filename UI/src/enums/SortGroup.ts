import ToDoTask from "../interfaces/ToDoTask";

enum SortGroup 
{
    Title = "title",
    Date = "dueDate",
}

export const SortGroupMethodMap = 
{
    [SortGroup.Title]: (a: ToDoTask, b: ToDoTask, isDesc: boolean) => 
        isDesc ?  b.title.localeCompare(a.title) : a.title.localeCompare(b.title),
    [SortGroup.Date]: (a: ToDoTask, b: ToDoTask, isDesc: boolean) => 
        isDesc 
            ? b.dueDate.getTime() - a.dueDate.getTime() 
            : a.dueDate.getTime() - b.dueDate.getTime()
}

enum SortDirection
{
    Ascending = "asc",
    Descending = "desc"
}

const SortDirectionSymbols =
{
    [SortDirection.Ascending]: "&#8593;",
    [SortDirection.Descending]: "&#8595;"
}
export { SortGroup, SortDirection, SortDirectionSymbols};