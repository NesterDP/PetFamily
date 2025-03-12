namespace PetFamily.Volunteers.Application.VolunteersManagement.Queries.GetFilteredPetsWithPaginationDapper;

public class Selector
{
    private const string WHERE = "\nWHERE ";
    private const string AND = "\nAND ";
    private int counter = 0;

    public string Select(string filter)
    {
        var head = " " + (counter == 0 ? WHERE : AND) +"(";
        var body = filter + ")";
        counter++;
        return head + body;
    }
}