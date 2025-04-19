namespace PetFamily.SharedKernel.Constants;

public static class InfrastructureConstants
{
    public const string DATABASE = "Database";
    public const int DEFAULT_RETRY_ATTEMPTS = 3;
    public const int DEFAULT_FIRST_RETRY_ATTEMPT_TIME = 5;
    public const int DEFAULT_SECOND_RETRY_ATTEMPT_TIME = 10;
    public const int DEFAULT_THIRD_RETRY_ATTEMPT_TIME = 15;
    public const int OUTBOX_TASK_WORKING_INTERVAL_IN_SECONDS = 1;
}