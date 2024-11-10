import { useState } from "react";
import "./Searchbar.css";
import ToDoTask from "../../interfaces/ToDoTask";
import { TaskStatus } from "../../enums/TaskStatus";
import { getAllByStatus } from "../../services/toDoService";
import { STATUS } from "../../enums/Status";

interface SearchbarProps
{
    taskData: ToDoTask[]
    setTaskData: React.Dispatch<React.SetStateAction<ToDoTask[]>>
    currentDisplayStatus: TaskStatus
    setIsSearching: React.Dispatch<React.SetStateAction<boolean>>
}

export default function Searchbar({taskData, setTaskData, currentDisplayStatus, setIsSearching}: SearchbarProps)
{
    const [query, setQuery] = useState("");

    const handleSearch = async (e: React.ChangeEvent<HTMLInputElement>) => 
    {
        const value = e.target.value ;
        setQuery(value);
        if(query.length <= 1)
        {
            setIsSearching(false);
            return;
        }
        setIsSearching(true);
        const inDisplayedTasks = searchAmongDisplayed(); 

        if(inDisplayedTasks && inDisplayedTasks.length)
        {
            setTaskData(inDisplayedTasks);
            return;
        }
        else
        {
            const response = await getAllByStatus(currentDisplayStatus, query);
            if(response.status == STATUS.Success && response.data)
                setTaskData(response.data);
        }
    };

    const searchAmongDisplayed = () => 
    {
        const filtered = taskData.filter(t =>
            t.title.toLowerCase().includes(query.toLowerCase()));
        return filtered;
    }

    return (
        <form 
            className="search-form"
            onSubmit={(e) => {
                e.preventDefault()
            }}
        >
            <input 
                type="search" 
                name="searchbar" 
                id="searchbar" 
                className="searchbar"
                value={query}
                placeholder="Search for a task"
                onChange={async (e) => await handleSearch(e)}
            />
        </form>
    );
}