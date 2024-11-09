import { useState } from "react";
import AsyncConfirmation from "../AsyncConfirmation/AsyncConfirmation"
import "./DeleteButton.css";

interface DeleteButtonProps
{
    confirmationCallback: () => Promise<unknown>
}

export default function DeleteButton({confirmationCallback}: DeleteButtonProps)
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
                callback={confirmationCallback}
            />
</>
    )
}