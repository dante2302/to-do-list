import React, { Children, PropsWithChildren } from "react";
import { TaskStatus } from "../../enums/TaskStatus";
import "./StatusHandler.css"
import AddButton from "../AddButton/AddButton";
interface props extends PropsWithChildren
{
    displayedTaskStatus: TaskStatus,
    setDisplayedTaskStatus: React.Dispatch<React.SetStateAction<TaskStatus>>
}

export default function StatusHandler({ displayedTaskStatus, setDisplayedTaskStatus, children}: props) {
    return (
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
            {children}
        </div>
    );
}