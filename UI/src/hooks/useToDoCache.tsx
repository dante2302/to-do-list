import { useEffect, useState } from "react";
import { TaskStatus } from "../enums/TaskStatus";
import ToDoTask from "../interfaces/ToDoTask";

type useToDoCacheReturnType = [
    Record<TaskStatus, ToDoTask[]>, 
    React.Dispatch<React.SetStateAction<Record<TaskStatus, ToDoTask[]>>>,
    (updatedTask: ToDoTask) => void,
    (updatedTask: ToDoTask) => void
]

export default function useToDoCache(): useToDoCacheReturnType
{
    const initialTaskCache = Object.values(TaskStatus).reduce((acc, status) => {
        acc[status] = [];
        return acc;
    }, {} as Record<TaskStatus, ToDoTask[]>);
    const [taskCache, setTaskCache] = useState<Record<TaskStatus, ToDoTask[]>>(initialTaskCache);

    const updateInAll = (prev: ToDoTask[], updatedTask: ToDoTask) =>
    {
        return prev.map(t => t.id == updatedTask.id ? updatedTask : t)
    }

    const cleanOnCompletion = (updatedTask: ToDoTask) =>
    {
        setTaskCache(prev => ({
            "completed": [...prev["completed"], updatedTask],
            "overdue": prev["overdue"].filter(t => t.id != updatedTask.id),
            "pending": prev["pending"].filter(t => t.id != updatedTask.id),
            "all": updateInAll(prev["all"], updatedTask)
        }));
    }

    const cleanAlreadyCompleted = (updatedTask: ToDoTask) =>
    {
        const isPending = updatedTask.dueDate >= new Date(Date.now());
        const forChange = isPending ? "pending" : "overdue";
        setTaskCache(prev => ({
            ...prev,
            "completed": prev["completed"].filter(t => t.id != updatedTask.id),
            [forChange]: [...prev[forChange], updatedTask],
            "all":  updateInAll(prev["all"], updatedTask)
        }))
    }

    return [taskCache, setTaskCache, cleanOnCompletion, cleanAlreadyCompleted];
}