import { PropsWithChildren } from "react"
import Modal from "../Modal/Modal";
import useLoadingSpinner from "../../hooks/useLoadingSpinner";
import "./AsyncConfirmation.css";

type props = PropsWithChildren & {
    message: string
    callback: () => Promise<any>,
    show: boolean,
    toggleShow: () => void
    yesMessage: string,
    noMessage: string,
    isSensitive: boolean
}

export default function AsyncConfirmation({
    message,
    toggleShow,
    show,
    callback,
    yesMessage,
    noMessage,
    isSensitive
}: props) 
{
    async function onSubmit() {
        await callbackWithLoading()
        toggleShow();
    }

    const [LoadingSpinner, callbackWithLoading, isLoading] = useLoadingSpinner(callback);
    return (
        show &&
        <Modal toggleModal={toggleShow}>
            <div className="confirmation-modal-content">
                <h1>{message}</h1>
                <div>
                    <button className={isSensitive ? "no-button" : "yes-button"}
                        onClick={() => onSubmit()}>
                        {
                            (!LoadingSpinner || !isLoading)
                                ?
                                yesMessage 
                                :
                                isLoading && <LoadingSpinner color="" />
                        }
                    </button>
                    <button className={`${isSensitive ? "sensitive" : ""} no-button `}
                        onClick={() => {
                            toggleShow();
                        }}>{noMessage}</button>
                </div>
            </div>
        </Modal>
    )
}