namespace PetFamily.Volunteers.Application.Queries.GetFilteredPetsWithPaginationDapper;

public class Selector
{
    private const string WHERE = "\nWHERE ";
    private const string AND = "\nAND ";
    private int counter = 0;

    public string Select(string filter)
    {
        string? head = " " + (counter == 0 ? WHERE : AND) +"(";
        string? body = filter + ")";
        counter++;
        return head + body;
    }
}