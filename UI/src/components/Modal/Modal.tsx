import { PropsWithChildren } from "react";
import "./Modal.css";
interface modalProps extends PropsWithChildren
{
    toggleModal: () => void
};

export default function Modal({toggleModal, children}: modalProps)
{
  return (     
    <div className='modal'>
      <div className='overlay' onClick={toggleModal}>
      </div>
        {children}
    </div>);
}