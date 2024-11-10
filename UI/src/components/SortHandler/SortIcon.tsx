import React from "react";
import "./styles/SortIcon.css";
interface SortIconProps
{
    className: string
    toggleSort: React.Dispatch<React.SetStateAction<boolean>>
}
export default function SortIcon({className, toggleSort}: SortIconProps)
{
    return <svg 
                onClick={() => toggleSort(prev => !prev)} 
                className={`${className} sort-icon`} 
                fill="#000000" 
                width="800px" 
                height="800px" 
                viewBox="0 0 36 36" 
                version="1.1" 
                preserveAspectRatio="xMidYMid meet" 
                xmlns="http://www.w3.org/2000/svg" 
                xmlnsXlink="http://www.w3.org/1999/xlink"
        >
        <path d="M28.54,13H7.46a1,1,0,0,1,0-2H28.54a1,1,0,0,1,0,2Z" className="clr-i-outline clr-i-outline-path-1"></path><path d="M21.17,19H7.46a1,1,0,0,1,0-2H21.17a1,1,0,0,1,0,2Z" className="clr-i-outline clr-i-outline-path-2"></path><path d="M13.74,25H7.46a1,1,0,0,1,0-2h6.28a1,1,0,0,1,0,2Z" className="clr-i-outline clr-i-outline-path-3"></path>
        <rect x="0" y="0" width="36" height="36" fillOpacity="0" />
    </svg>
}