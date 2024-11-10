import { useEffect, useState } from "react";
import useFormChange from "../../hooks/useFormChange";
import DatePicker from "react-datepicker";
import "./AddForm.css";
import 'react-datepicker/dist/react-datepicker.css';
import * as toDoService from "../../services/toDoService";
import { ToDoSubmission } from "../../interfaces/ToDoSubmission";
import useLoadingSpinner from "../../hooks/useLoadingSpinner";
import { getNearest10MinuteInterval } from "../../utils/dateUtils";
import { STATUS } from "../../enums/Status";
import ToDoTask from "../../interfaces/ToDoTask";

interface AddFormProps 
{
    toggleModal: () => void,
    cacheNewTask: (newTask: ToDoTask) => void,
    displayNewTask: (newTask: ToDoTask) => void,
}

export default function AddForm({ toggleModal, cacheNewTask, displayNewTask }: AddFormProps) {
    const initialState: ToDoSubmission = {
        title: "",
        dueDate: getNearest10MinuteInterval()
    };

    const [formState, setFormState] = useState(initialState);
    const [validationError, setValidationError] = useState(false);
    const [fetchError, setFetchError] = useState(false);
    const [hasBlurred, setHasBlurred] = useState(false);

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

    const checkError = () => 
    {
        if(!formState.title)
            setValidationError(true)
        else
            setValidationError(false)
    }

    useEffect(() => {
        checkError();
    }, [formState])

    const changeHandler = useFormChange(setFormState);

    const filterTime = (time: Date) => 
    {
        const currentDate = new Date();
        const selectedDate = new Date(time);
        return selectedDate >= currentDate;
    }

    const submitHandler = async () => 
    {
        const serviceResponse = await toDoService.create(formState);
        if(serviceResponse.status == STATUS.Error || !serviceResponse.data)
        {
            setFetchError(true);
            return;
        }
        const newTask = serviceResponse.data;
        displayNewTask(newTask);
        cacheNewTask(newTask);
        toggleModal();
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
                    <span className="required">*</span>Title
                </label>
                <input
                    type="text"
                    name="title"
                    value={formState.title}
                    onChange={(e) => changeHandler(e)}
                    className="title-input"
                    onBlur={() => setHasBlurred(true)}
                />
                {(validationError && hasBlurred )&& <p className="validation-message">Title is required</p>}
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
                {fetchError && <div>SOmething went wrong..</div>}
            </form>
        </div>
    );
}