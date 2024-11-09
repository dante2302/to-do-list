export default function useFormChange<T>(setFormState: React.Dispatch<React.SetStateAction<T>>)
{
    const changeHandler = (e: React.ChangeEvent<HTMLInputElement>) => {
        e.preventDefault()

        setFormState(state => ({
            ...state, [e.target.name]: `${e.target.value}`
        }))
    };
    return changeHandler;
}