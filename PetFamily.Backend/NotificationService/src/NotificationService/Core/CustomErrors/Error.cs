namespace NotificationService.Core.CustomErrors;

public record Error
{
    private const string SEPARATOR = "||";

    private string Code { get; }

    private string Message { get; }

    private ErrorType Type { get; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? InvalidField { get; }

    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public static Error Validation(string code, string message, string? invalidField = null) =>
        new Error(code, message, ErrorType.Validation, invalidField);

    public static Error NotFound(string code, string message) =>
        new Error(code, message, ErrorType.NotFound);

    public static Error Failure(string code, string message) =>
        new Error(code, message, ErrorType.Failure);

    public static Error Conflict(string code, string message) =>
        new Error(code, message, ErrorType.Conflict);

    public ErrorList ToErrorList() => new ErrorList([this]);

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        string[] parts = serialized.Split("||");
        if (parts.Length < 3)
            throw new ArgumentException($"Invalid error format: {serialized}");

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
            throw new ArgumentException($"Invalid error format: {serialized}");

        return new Error(parts[0], parts[1], type);
    }
}