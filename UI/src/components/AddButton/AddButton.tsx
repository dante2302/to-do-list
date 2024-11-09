import { useState } from "react";
import "./AddButton.css";
import Modal from "../Modal/Modal";
import AddForm from "../AddForm/AddForm";
import ToDoTask from "../../interfaces/ToDoTask";

interface AddButtonProps 
{
    cacheNewTask: (newTask: ToDoTask) => void,
    displayNewTask: (newTask: ToDoTask) => void,
}

export default function AddButton({
    cacheNewTask, 
    displayNewTask
}: AddButtonProps)
{
    const [showModal, setShowModal] = useState(false);
    const toggleModal = () => {setShowModal(prev => !prev)};
    return (
        <>
        {showModal && 
            <Modal toggleModal={toggleModal}>
                <AddForm 
                    toggleModal={toggleModal}
                    cacheNewTask={cacheNewTask}
                    displayNewTask={displayNewTask}
                />
            </Modal>
        }
            <button 
                className="add-btn"
                onClick={toggleModal}
            >
                <>&#43;</>
            </button>
        </>
    )
}