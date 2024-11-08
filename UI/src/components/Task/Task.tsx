import ToDoTask from "../../interfaces/ToDoTask";
import "./Task.css";

interface TaskProps
{
    taskData: ToDoTask;
    setTaskData: React.Dispatch<React.SetStateAction<ToDoTask>>;
}

export default function Task({ taskData, setTaskData }: TaskProps)
{
    const toggleCompleted = () => 
    {
        setTaskData(prev => ({
            ...prev, 
            isCompleted: !prev.isCompleted
        }))
    }

    return (
        <div className="task">
            <form>
                <input 
                    type="checkbox" 
                    checked={taskData.isCompleted} 
                    onChange={toggleCompleted}
                />
            </form>
            <h2>{taskData.title}</h2>
            <div className="date-time-container">
                <span>{taskData.dueDate.toLocaleDateString()}</span>
                <span>{taskData.dueDate.toLocaleTimeString()}</span>
            </div>
        </div>
    );
}