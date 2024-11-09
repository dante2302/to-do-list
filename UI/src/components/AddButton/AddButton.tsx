import { useState } from "react";
import "./AddButton.css";
import Modal from "../Modal/Modal";
import AddForm from "../AddForm/AddForm";

export default function AddButton()
{
    const [showModal, setShowModal] = useState(false);
    const toggleModal = () => {setShowModal(prev => !prev)};
    return (
        <>
        {showModal && 
            <Modal toggleModal={toggleModal}>
                <AddForm toggleModal={toggleModal}/>
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