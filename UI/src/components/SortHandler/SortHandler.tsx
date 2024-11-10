import { useState } from "react";
import SortIcon from "./SortIcon";
import { SortGroup } from "../../enums/SortGroup";
import "./styles/SortHandler.css";
import useOutsideClick from "../../hooks/useOutsideClick";

export default function SortHandler() {
    const [showSortOptions, setShowSortOptions] = useState(false);
    const [currentSort, setCurrentSort] = useState(SortGroup.Title);
    const outsideClickRef = useOutsideClick<HTMLDivElement>(() => setShowSortOptions(false));

    const handleSortClick= async (sortGroup: SortGroup) => 
    {
        setCurrentSort(sortGroup);
    }

    return (
        <div className="sort-outer" ref={outsideClickRef}>
            <SortIcon
                className={`${showSortOptions ? "active" : ""}`}
                toggleSort={setShowSortOptions}
            />
            <div className={`${showSortOptions ? "active" : ""} sort-options`}>
                <span className="sort-options-heading">Sort by</span>
                {Object.values(SortGroup).map(sg =>
                    <span
                        onClick={async () => await handleSortClick(sg)}
                        key={sg}
                        className={`${currentSort == sg ? "active" : ""} sort-option`}
                    >
                        {sg}{currentSort == sg ? <span>&#10003;</span> : ""}
                    </span>
                )}
            </div>
        </div>
    )
};