import "./Searchbar.css";

export default function Searchbar()
{
    const handleSearch = () => {
    };
    return (
        <form 
            className="search-form"
            onSubmit={(e) => {
                e.preventDefault()
            }}
        >
            <input 
                type="search" 
                name="searchbar" 
                id="searchbar" 
                className="searchbar"
                placeholder="Search for a task"
                onChange={(e) => handleSearch}
            />
        </form>
    );
}