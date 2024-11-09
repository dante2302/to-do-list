import { useEffect, useState } from "react";
import { TaskStatus } from "../../enums/TaskStatus";
import ToDoTask from "../../interfaces/ToDoTask";
import { getAllByStatus } from "../../services/toDoService";
import { STATUS } from "../../enums/Status";
import Task from "../Task/Task";
import useLocalStorage from "../../hooks/useLocalStorage";
import "./TaskHandler.css";
import Searchbar from "../Searchbar/Searchbar";
import StatusHandler from "../StatusHandler/StatusHandler";

export default function TaskHandler() {
    const [displayedTaskStatus, setDisplayedTaskStatus] =
        useState<TaskStatus>(TaskStatus.Pending);

    const [taskData, setTaskData] =
        useLocalStorage<ToDoTask[]>(`${displayedTaskStatus}Tasks`, []);

    const [hasError, setHasError] = useState(false);

    const updateTask = (updatedTask: ToDoTask) => {
        setTaskData(prevTasks =>
            prevTasks.map(task =>
                task.id === updatedTask.id ? updatedTask : task
            )
        );
    }

    useEffect(() => {
        (async () => {
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
        <div className="outer-wrap">
            <div className="outer-container">
                <h1>TransferMate To Do List</h1>
                <div className="upper-container">
                    <Searchbar />
                    <StatusHandler 
                        displayedTaskStatus={displayedTaskStatus} 
                        setDisplayedTaskStatus={setDisplayedTaskStatus}
                    />
                </div>
                <div className="lower-container">
                    {taskData.length
                        ?
                        taskData.map(task =>
                            <Task
                                key={task.id}
                                taskData={task}
                                updateTask={updateTask}
                            />
                        )
                        :
                        <p 
                            className="no-tasks"
                        >You have no {displayedTaskStatus == "all" ? "" : displayedTaskStatus } tasks.</p>
                    }
                </div>
            </div>
        </div>
    );
}