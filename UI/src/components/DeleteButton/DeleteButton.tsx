import { useState } from "react";
import AsyncConfirmation from "../AsyncConfirmation/AsyncConfirmation"
import "./DeleteButton.css";
import ToDoTask from "../../interfaces/ToDoTask";

interface DeleteButtonProps
{
    confirmationCallback: (task: ToDoTask) => Promise<unknown>
    taskData: ToDoTask
}

export default function DeleteButton({confirmationCallback, taskData}: DeleteButtonProps)
{
    const [showConfirmation, setShowConfirmation] = useState(false);
    const toggleShowConfirmation = () => {setShowConfirmation(!showConfirmation)};

    return(
        <>
        <button 
            onClick={toggleShowConfirmation}
            className="delete-button"
        >
            &times;
        </button>
            <AsyncConfirmation
                message="Are you sure you want to delete this task?"
                yesMessage="Delete"
                noMessage="Cancel"
                toggleShow={toggleShowConfirmation}
                show={showConfirmation}
                isSensitive={true}
                callback={() => confirmationCallback(taskData)}
            />
</>
    )
}