import { useState } from "react"
import { ClipLoader } from "react-spinners"
import { LoaderSizeMarginProps, LoaderSizeProps } from "react-spinners/helpers/props";

type Callback = (...args: any[]) => Promise<void>
type ErrorCallback = (error: any | unknown) => void;
export type LoadingSpinner = ({ color, size }: LoaderSizeProps) => JSX.Element;

function useLoadingSpinner(callback: Callback, errorCallback?: ErrorCallback): [LoadingSpinner, Callback, boolean]
{
  const [isLoading,setLoading] = useState(false)

  const callbackWithLoading = async (...args: any[]) => {
    try{
      setLoading(true);
      await callback(...args);
    }
    catch(error: unknown){
      if(errorCallback)errorCallback(error)
    }
    finally{
        setLoading(false);
    }
  }
  
  const LoadingSpinner = (props: LoaderSizeMarginProps) => {
    return <ClipLoader {...props}/> 
  }

  return [LoadingSpinner, callbackWithLoading, isLoading]
}

export default useLoadingSpinner