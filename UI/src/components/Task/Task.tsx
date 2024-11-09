import ToDoTask from "../../interfaces/ToDoTask";
import { UpdateTask } from "../../interfaces/UpdateTask";
import * as toDoService from "../../services/toDoService";
import "./Task.css";

interface TaskProps
{
    taskData: ToDoTask;
    updateTask: UpdateTask;
}

export default function Task({ taskData, updateTask }: TaskProps)
{
    const toggleCompleted = async () => 
    {
        const updated = { ...taskData, isCompleted: !taskData.isCompleted };
        updateTask(updated);
        await toDoService.update(updated);
    }

    return (
        <div className="task">
            <form>
                <input 
                    type="checkbox" 
                    checked={taskData.isCompleted} 
                    onChange={async () => await toggleCompleted()}
                />
            </form>
            <div className="inner-task">
                <span
                    className={taskData.isCompleted ? "striked" : "not-striked"}
                ></span>
                <h2
                >{taskData.title}</h2>
                <div className="date-time-container">
                    <span>{new Date(taskData.dueDate).toLocaleDateString()}</span>
                    <span>{new Date(taskData.dueDate).toLocaleTimeString()}</span>
                </div>
            </div>
        </div>
    );
}