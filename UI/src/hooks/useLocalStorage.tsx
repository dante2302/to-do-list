import { useEffect, useState } from "react";

export default function useLocalStorage<T>(key: string, defaultValue: T): 
    [T, React.Dispatch<React.SetStateAction<T>>]
{
    const [state, setState] = useState<T>(() => getStorageValue(key, defaultValue));

    useEffect(() => {setStorageValue(key, state)}, [state]);

    return [state, setState];
}

function setStorageValue<T>(key: string, state: T)
    {
        localStorage.setItem(key, JSON.stringify(state))
    }

function getStorageValue<T>(key: string, defaultValue?: T) {
        const storage = localStorage.getItem(key);
        const initial = storage !== null ? JSON.parse(storage) : defaultValue;
        return initial; 
}