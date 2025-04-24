namespace PetFamily.SharedKernel.CustomErrors;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsRequired(string? propertyName = null)
        {
            string text = propertyName ?? "value";
            return Error.Validation("value.is.required", $" {text} is required");
        }

        public static Error ValueIsInvalid(string? propertyName = null)
        {
            string text = propertyName ?? "value";
            return Error.Validation("value.is.invalid", $" {text} is invalid");
        }

        public static Error ValueIsInvalid(string message, string propertyName)
        {
            return Error.Validation("value.is.invalid", $" {propertyName} is invalid, {message}");
        }

        public static Error ValueNotFound(Guid? id = null)
        {
            string text = id == null ? string.Empty : $" for id {id}";
            return Error.NotFound("record.not.found", $"record not found{text}");
        }

        public static Error ValueNotFound(string? message)
        {
            return Error.NotFound("record.not.found", $"record {message} not found");
        }

        public static Error ValueNotFound(string? message, bool wholeMessage)
        {
            return Error.NotFound("record.not.found", $"{message}");
        }

        public static Error Conflict(string? message)
        {
            return Error.Conflict("logic.conflict", $"{message}");
        }

        public static Error Failure(string code, string? message)
        {
            return Error.Failure(code, $"{message}");
        }

        public static Error Failure(string? message)
        {
            return Error.Failure("server.internal", $"{message}");
        }

        public static Error AlreadyExist(string? message)
        {
            string text = message ?? "record";
            return Error.Validation("record.already.exist", $"{text} already exist");
        }

        public static Error LengthIsInvalid(int lessThen, string? propertyName = null)
        {
            string text = propertyName ?? string.Empty;
            return Error.Validation(
                "value.length.invalid",
                $"{text} length is invalid. Maximum length is {lessThen}.");
        }

        public static Error ExpiredToken()
        {
            return Error.Validation("token.is.expired", "your token is expired");
        }

        public static Error InvalidToken()
        {
            return Error.Validation("token.is.invalid", "your token is invalid");
        }
    }
}