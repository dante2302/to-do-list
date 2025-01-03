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
import useLoadingSpinner from "../../hooks/useLoadingSpinner";
import * as toDoService from "../../services/toDoService";
import SortHandler from "../SortHandler/SortHandler";
import { SortDirection, SortGroup } from "../../enums/SortGroup";

export default function TaskHandler() {
    const [isInitialRender, setIsInitialRender] = useState(
        Object.fromEntries(
        Object.values(TaskStatus).map((key) => [key, true])
    ));

    const [displayedTaskStatus, setDisplayedTaskStatus] =
        useLocalStorage<TaskStatus>("LastDisplayedTaskStatus", TaskStatus.Pending);

    const [currentSort, setCurrentSort] = useLocalStorage("LastSort", SortGroup.Title);
    const [currentSortDirection, setCurrentSortDirection] = 
        useLocalStorage("LastSortDirection", SortDirection.Descending)

    const [
        taskCache,
        setTaskCache,
        cleanOnCompletion,
        cleanAlreadyCompleted,
        cacheNew,
        removeFromCache
    ] = useToDoCache();

    const [taskData, setTaskData] =
        useState<ToDoTask[]>([]);

    const [hasError, setHasError] = useState(false);
    const [isSearching, setIsSearching] = useState(false);

    useEffect(() => {fetchTasksWithLoading();}, [displayedTaskStatus, isSearching]);
    const updateTaskCompletion = (updatedTask: ToDoTask) => {
        setTaskData(prevTasks => 
            prevTasks.map(task => 
                task.id === updatedTask.id ? updatedTask : task
        ));

        updatedTask.isCompleted
            ? cleanOnCompletion(updatedTask)
            : cleanAlreadyCompleted(updatedTask);
    }

    const displayNewTask = (newTask: ToDoTask) => {
        if (displayedTaskStatus == "all" || displayedTaskStatus == "pending")
            setTaskData(prevTasks => ([
                ...prevTasks,
                newTask
            ]));
    }

    const deleteTask = async (task: ToDoTask) => {
        setTaskData(taskData.filter(t => t.id != task.id));
        removeFromCache(task);
        toDoService._delete(task.id);
    }

    const fetchTasks = () => {
        if(isSearching) return;

        const cachedTasks = taskCache[displayedTaskStatus];
        // Javascript way of logical XOR
        // of cachedTasks && cachedTasks.length XOR !isInitialRender
        if ((!!cachedTasks && !!cachedTasks.length)
            && (!isInitialRender[displayedTaskStatus])
        ) 
        {
                setTaskData(cachedTasks);
                return;
        }
        else {
            (async () => {
                const serviceResponse = await getAllByStatus(
                    displayedTaskStatus,
                    undefined,
                    currentSort,
                    currentSortDirection
                );
                if (serviceResponse.status == STATUS.Error) {
                    setHasError(true);
                    return;
                }

                if (serviceResponse.data && serviceResponse.data.length) {
                    setTaskData(serviceResponse.data);
                    setTaskCache(prev => ({
                        ...prev,
                        [displayedTaskStatus]: serviceResponse.data
                    }))
                }

                else
                    setTaskData([]);
            })();
        }
        setIsInitialRender(obj => ({...obj, [displayedTaskStatus]: false}));
    }

    const [LoadingSpinner, fetchTasksWithLoading, isLoading] = useLoadingSpinner(fetchTasks);
    return (
        <>
            <div className="outer-wrap">
                <div className="outer-container">
                    <div className="upper-container">
                        <h1>TransferMate To Do List</h1>
                        <div>

                        <Searchbar 
                            taskData={taskData}
                            setTaskData={setTaskData}
                            currentDisplayStatus={displayedTaskStatus}
                            setIsSearching={setIsSearching}
                        />
                        </div>
                        <StatusHandler
                            displayedTaskStatus={displayedTaskStatus}
                            setDisplayedTaskStatus={setDisplayedTaskStatus}
                        >
                            <AddButton
                                cacheNewTask={cacheNew}
                                displayNewTask={displayNewTask}
                            />
                            <SortHandler
                                currentSort={currentSort}
                                currentSortDirection={currentSortDirection}
                                setCurrentSort={setCurrentSort}
                                setCurrentSortDirection={setCurrentSortDirection}
                                setTasksData={setTaskData}
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
                                        deleteTask={deleteTask}
                                    />
                                )
                                :
                                isLoading ?
                                    <LoadingSpinner size={45} />
                                    :
                                    <p
                                        className="no-tasks"
                                    >You have no {displayedTaskStatus == "all" ? "" : displayedTaskStatus} tasks.</p>
                            }
                        </div>
                    }
                </div>
            </div>
        </>
    );
}