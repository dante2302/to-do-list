import { useState } from "react";
import "./Searchbar.css";
import useFormChange from "../../hooks/useFormChange";
import ToDoTask from "../../interfaces/ToDoTask";

interface SearchbarProps
{
    taskData: ToDoTask[]
    setTaskData: React.Dispatch<React.SetStateAction<ToDoTask[]>>
}

export default function Searchbar({taskData, setTaskData}: SearchbarProps)
{
    const [query, setQuery] = useState("");

    const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => 
    {
        const value = e.target.value ;
        setQuery(value);
        const inDisplayedTasks = searchAmongDisplayed(); 

        if(inDisplayedTasks && inDisplayedTasks.length)
        {
            setTaskData(inDisplayedTasks);
        }
        else
        {
            
        }
    };

    const searchAmongDisplayed = () => 
    {
        return taskData.filter(t =>
            t.title.toLowerCase().includes(query.toLowerCase()));
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
                onChange={(e) => handleSearch(e)}
            />
        </form>
    );
}