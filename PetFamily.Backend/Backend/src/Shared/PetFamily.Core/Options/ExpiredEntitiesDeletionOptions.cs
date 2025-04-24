namespace PetFamily.Core.Options;

public class ExpiredEntitiesDeletionOptions
{
    public const string EXPIRED_ENTITIES_DELETION = "ExpiredEntitiesDeletion";

    public int WorkHoursInterval { get; init; }

    public int EntityExpiredDaysTime { get; init; }
}