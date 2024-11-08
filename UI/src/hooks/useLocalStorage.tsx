import { useEffect, useState } from "react";

export default function useLocalStorage<T>(key: string, defaultValue: T): 
    [T, React.Dispatch<React.SetStateAction<T>>, (key: string) => T]
{
    const [state, setState] = useState<T>(() => getStorageValue(key));

    useEffect(() => {
        localStorage.setItem(key, JSON.stringify(state))
    }, [state]);

    function getStorageValue(key: string) {
        const storage = localStorage.getItem(key);
        const initial = storage !== null ? JSON.parse(storage) : defaultValue;
        return initial; 
    }

    return [state, setState, getStorageValue];
}