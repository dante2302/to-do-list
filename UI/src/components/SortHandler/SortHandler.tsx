import { useState } from "react";
import SortIcon from "./SortIcon";
import { SortDirection, SortDirectionSymbols, SortGroup, SortGroupMethodMap } from "../../enums/SortGroup";
import "./styles/SortHandler.css";
import useOutsideClick from "../../hooks/useOutsideClick";
import ToDoTask from "../../interfaces/ToDoTask";

interface SortHandlerProps
{
    setTasksData: React.Dispatch<React.SetStateAction<ToDoTask[]>>
    currentSort: SortGroup
    currentSortDirection: SortDirection
    setCurrentSort:React.Dispatch<React.SetStateAction<SortGroup>> 
    setCurrentSortDirection:React.Dispatch<React.SetStateAction<SortDirection>> 
}

export default function SortHandler({ 
    setTasksData, 
    currentSort, 
    setCurrentSort,
    currentSortDirection,
    setCurrentSortDirection
}: SortHandlerProps) {
    const [showSortOptions, setShowSortOptions] = useState(false);
    const outsideClickRef = useOutsideClick<HTMLDivElement>(() => setShowSortOptions(false));

    const handleSortClick= (sortGroup: SortGroup) => 
    {
        if(sortGroup == currentSort) return;
        setCurrentSort(sortGroup);
        setTasksData(currentTasks => 
            currentTasks.sort((a, b) => 
                SortGroupMethodMap[sortGroup](a, b, currentSortDirection == "desc"))
        );
    }

    const handleSortDirClick = (sortDir: SortDirection) => 
    {
        if(sortDir == currentSortDirection) return;
        setCurrentSortDirection(sortDir);
        setTasksData(currentTasks => 
            currentTasks.sort((a, b) => 
                SortGroupMethodMap[currentSort](a, b, sortDir == "desc"))
        );
    }

    return (
        <div className="sort-outer" ref={outsideClickRef}>
            <SortIcon
                className={`${showSortOptions ? "active" : ""}`}
                toggleSort={setShowSortOptions}
            />
            <div className={`${showSortOptions ? "active" : ""} sort-options`}>
                <div>
                    <span className="sort-options-heading">Sort by</span>
                    {Object.entries(SortDirection).map(sdEntry => 
                        <span 
                            className={`${sdEntry[1] == currentSortDirection ? "active" : ""} sort-dir`}
                            key={sdEntry[0]}
                            onClick={() => handleSortDirClick(sdEntry[1])}
                        >
                            {sdEntry[0]}
                            <span 
                                dangerouslySetInnerHTML={{__html: SortDirectionSymbols[sdEntry[1]]} }
                                className="dir-symbol"
                            >
                            </span>
                        </span>
                    )}
                </div>
                {Object.values(SortGroup).map(sg =>
                    <span
                        onClick={() => handleSortClick(sg)}
                        key={sg}
                        className={currentSort == sg ? "active" : ""}
                    >
                        {sg}{currentSort == sg ? <span>&#10003;</span> : ""}
                    </span>
                )}
            </div>
        </div>
    )
};