import { useEffect, useRef } from 'react';

export default function useOutsideClick<T extends Element>(action: () => any){
  const ref = useRef<T>(null)

  useEffect(() => {
    const handleClick = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        action();
      }
    };

    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, [action]);

  return ref;
}