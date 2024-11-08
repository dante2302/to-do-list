import { useEffect, useState } from "react";
import { TaskStatus } from "../../enums/TaskStatus";
import ToDoTask from "../../interfaces/ToDoTask";
import { getAllByStatus } from "../../services/toDoService";
import { STATUS } from "../../enums/Status";
import Task from "../Task/Task";
import useLocalStorage from "../../hooks/useLocalStorage";
import Searchbar from "../Searchbar/Searchbar";

export default function TaskHandler() {
    const [displayedTaskStatus, setDisplayedTaskStatus] =
        useState<TaskStatus>(TaskStatus.Pending);

    const [taskData, setTaskData, getStorageValue] = 
        useLocalStorage<ToDoTask[]>(`${displayedTaskStatus}Tasks`, []);

    const [hasError, setHasError] = useState(false);
    const [hasChange, setHasChange] = useState(false);

    const updateTask = (updatedTask: ToDoTask) => {
        setTaskData(prevTasks =>
            prevTasks.map(task =>
                task.id === updatedTask.id ? updatedTask : task
            )
        );
    }

    useEffect(() => {
        (async () => {
            const upcoming = getStorageValue(`${displayedTaskStatus}Tasks`);
            if(!hasChange && upcoming && upcoming.length) 
            {
                setTaskData(upcoming);
                return;
            }
            const serviceResponse = await getAllByStatus(displayedTaskStatus);
            if (serviceResponse.status == STATUS.Error) {
                setHasError(true);
                return;
            }
            console.log(serviceResponse.data);
            setTaskData(serviceResponse.data || []);
        })();
    }, [displayedTaskStatus]);


    return (
        <div className="outer-container">
            <Searchbar />
            <div className="status-container">
                {Object.values(TaskStatus).map(ts =>
                    <span 
                        className={displayedTaskStatus == ts ? "active" : ""}
                        key={ts}
                        onClick={() => setDisplayedTaskStatus(ts)}
                    >
                        {ts}
                    </span>
                )}
            </div>
            {taskData && taskData.length 
                ?
                taskData.map(task => 
                    <Task 
                        key={task.id}
                        taskData={task} 
                        updateTask={updateTask}
                        setChange={setHasChange}
                    />
                )
                :
                <p>You have no {displayedTaskStatus} tasks</p>
        }
        </div>   
    );
}