export default function Searchbar()
{
    const handleSearch = () => {};
    return (
        <form>
            <input 
                type="search" 
                name="searchbar" 
                id="searchbar" 
                onChange={handleSearch}
            />
        </form>
    );
}