import { useState } from "react";
import SortIcon from "./SortIcon";
import { SortGroup } from "../../enums/SortGroup";
import "./styles/SortHandler.css";

export default function SortHandler() {
    const [showSortOptions, setShowSortOptions] = useState(false);

    return (
        <div className="sort-outer">
            <SortIcon
                className={`${showSortOptions ? "active" : ""}`}
                toggleSort={setShowSortOptions}
            />
            <div className={`${showSortOptions ? "active" : ""} sort-options`}>
                {Object.values(SortGroup).map(sg =>
                    <span
                        key={sg}
                    >
                        {sg}
                    </span>
                )}
            </div>
        </div>
    )
};