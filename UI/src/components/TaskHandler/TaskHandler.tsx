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
import useToDoCache from "../../hooks/useToDoCache";
import AddButton from "../AddButton/AddButton";

export default function TaskHandler() {
    const [displayedTaskStatus, setDisplayedTaskStatus] =
        useLocalStorage<TaskStatus>("LastDisplayedTaskStatus", TaskStatus.Pending);

    const [
        taskCache, 
        setTaskCache, 
        cleanOnCompletion,
        cleanAlreadyCompleted,
        cacheNewTask
    ] = useToDoCache();

    const [taskData, setTaskData] =
        useState<ToDoTask[]>([]);

    const [hasError, setHasError] = useState(false);

    const updateTaskCompletionList = (list: ToDoTask[], updatedTask: ToDoTask) =>
    {
        const updatedList = list.map(task => 
            task.id === updatedTask.id ? updatedTask : task
        );
        return updatedList;
    }

    const updateTaskCompletion = (updatedTask: ToDoTask) => {
        setTaskData(prevTasks => updateTaskCompletionList(prevTasks, updatedTask));
        updatedTask.isCompleted 
            ? cleanOnCompletion(updatedTask) 
            : cleanAlreadyCompleted(updatedTask);
    }

    const displayNewTask = (newTask: ToDoTask) => 
    {
        if(displayedTaskStatus == "all" || displayedTaskStatus == "pending")
            setTaskData(prevTasks => ([
                ...prevTasks,
                newTask
            ]));
    }

    // useEffect(() => {
    //     console.log(taskData);
    // }, [taskData])

    useEffect(() => {
        const cachedTasks = taskCache[displayedTaskStatus];
        if (cachedTasks && cachedTasks.length) 
        {
            setTaskData(cachedTasks);
        }
        else 
        {
            (async () => {
                const serviceResponse = await getAllByStatus(displayedTaskStatus);
                if (serviceResponse.status == STATUS.Error) {
                    setHasError(true);
                    return;
                }
                if(serviceResponse.data && serviceResponse.data.length)
                {
                    setTaskData(serviceResponse.data);
                    setTaskCache(prev => ({
                        ...prev,
                        [displayedTaskStatus]: serviceResponse.data
                    }))
                }
            })();
        }
    }, [displayedTaskStatus]);


    return (
        <div className="outer-wrap">
            <div className="outer-container">
                <div className="upper-container">
                <h1>TransferMate To Do List</h1>
                    <Searchbar />
                    <StatusHandler 
                        displayedTaskStatus={displayedTaskStatus} 
                        setDisplayedTaskStatus={setDisplayedTaskStatus}
                    >
                        <AddButton 
                            cacheNewTask={cacheNewTask}
                            displayNewTask={displayNewTask}
                        />
                    </StatusHandler>
                </div>
                {hasError 
                    ? 
                    <h2>Something Went Wrong. Try Again Later</h2>
                    :
                <div className="lower-container">
                    {taskData.length
                        ?
                        taskData.map(task =>
                            <Task
                                key={task.id}
                                taskData={task}
                                updateTask={updateTaskCompletion}
                            />
                        )
                        :
                        <p 
                            className="no-tasks"
                        >You have no {displayedTaskStatus == "all" ? "" : displayedTaskStatus } tasks.</p>
                    }
                </div>
                }
            </div>
        </div>
    );
}