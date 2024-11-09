import { useEffect, useState } from "react";
import useFormChange from "../../hooks/useFormChange";
import DatePicker from "react-datepicker";
import "./AddForm.css";
import 'react-datepicker/dist/react-datepicker.css';
import * as toDoService from "../../services/toDoService";
import { ToDoSubmission } from "../../interfaces/ToDoSubmission";
import useLoadingSpinner from "../../hooks/useLoadingSpinner";

interface AddFormProps {
    toggleModal: () => void
}

export default function AddForm({ toggleModal }: AddFormProps) {
    const initialState: ToDoSubmission = {
        title: "",
        dueDate: new Date(Date.now() + 5 * 600000)
    };

    const [formState, setFormState] = useState(initialState);
    const [validationError, setValidationError] = useState(true);

    const dateChangeHandler = (date: Date | null) => 
    {
        if(date)
        {
            setFormState(prev => ({
                ...prev,
                dueDate: date
            }))
        }
    }

    useEffect(() => {
        if(!formState.title)
            setValidationError(true)
        else
            setValidationError(false)
    }, [formState])

    const changeHandler = useFormChange(setFormState);
    const filterTime = (time: Date) => 
    {
        const currentDate = new Date();
        const selectedDate = new Date(time);

        // Only allow times after the current time if the date is today
        return selectedDate >= currentDate;
    }

    const submitHandler = async () => 
    {
        await toDoService.create(formState);
    }

    const [Spinner, submitWithSpinner, isLoading] = useLoadingSpinner(submitHandler);
    return (
        <div className="add-modal-container">
            <h2>Create a task</h2>
            <form
                className="add-form"
                onSubmit={(e) => e.preventDefault()}
            >
                <div className="input-wrap">

                <label htmlFor="title">
                    Title
                </label>
                <input
                    type="text"
                    name="title"
                    value={formState.title}
                    onChange={(e) => changeHandler(e)}
                    className="title-input"
                />
                </div>
                <div className="input-wrap date-input-wrap">

                <label htmlFor="date-input">
                    Date & Time
                </label>
                <DatePicker 
                    selected={formState.dueDate}
                    onChange={(date) => dateChangeHandler(date)}
                    minDate={new Date()}
                    showDateSelect
                    showTimeSelect
                    placeholderText="Choose a date"
                    timeIntervals={10}
                    timeFormat="HH:mm"
                    filterTime={filterTime}
                    disabledKeyboardNavigation
                    className="date-input"
                    dateFormat={"dd/MM/yy H:mm"}
                />
                </div>
                <div className="button-container">
                <button 
                    className="create-task-btn" 
                    onClick={async () => await submitWithSpinner()}
                    disabled={validationError}
                >{isLoading ? <Spinner size={15} /> :  "Create"}</button>
                <button className="cancel-button" onClick={toggleModal}>Cancel</button>
                </div>
            </form>
        </div>
    );
}