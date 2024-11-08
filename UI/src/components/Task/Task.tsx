import ToDoTask from "../../interfaces/ToDoTask";
import { UpdateTask } from "../../interfaces/UpdateTask";
import * as toDoService from "../../services/toDoService";
import "./Task.css";

interface TaskProps
{
    taskData: ToDoTask;
    updateTask: UpdateTask;
    setChange: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function Task({ taskData, updateTask, setChange }: TaskProps)
{
    const toggleCompleted = async () => 
    {
        const updated = {...taskData, isCompleted: !taskData.isCompleted};
        updateTask(updated);
        await toDoService.update(updated);
    }
    console.log(typeof taskData.dueDate)
    return (
        <div className="task">
            <form>
                <input 
                    type="checkbox" 
                    checked={taskData.isCompleted} 
                    onChange={async () => await toggleCompleted()}
                />
            </form>
            <h2>{taskData.title}</h2>
            <div className="date-time-container">
                {/* <span>{taskData.dueDate.toLocaleDateString()}</span> */}
                <span>{taskData.dueDate.toLocaleString()}</span>
            </div>
        </div>
    );
}