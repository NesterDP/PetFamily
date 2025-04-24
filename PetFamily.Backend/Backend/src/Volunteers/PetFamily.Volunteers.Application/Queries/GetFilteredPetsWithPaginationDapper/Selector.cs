namespace PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPaginationDapper;

public class Selector
{
    private const string WHERE = "\nWHERE ";
    private const string AND = "\nAND ";
    private int _counter;

    public string Select(string filter)
    {
        string head = " " + (_counter == 0 ? WHERE : AND) + "(";
        string body = filter + ")";
        _counter++;
        return head + body;
    }
}