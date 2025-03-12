namespace PetFamily.Core.CustomErrors;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsRequired(string? propertyName = null)
        {
            var text = propertyName ?? "value";
            return Error.Validation("value.is.required", $" {text} is required");
        }

        public static Error ValueIsInvalid(string? propertyName = null)
        {
            var text = propertyName ?? "value";
            return Error.Validation("value.is.invalid", $" {text} is invalid");
        }

        public static Error ValueNotFound(Guid? id = null)
        {
            var text = id == null ? "" : $" for id {id}";
            return Error.NotFound("record.not.found", $"record not found{text}");
        }
        
        public static Error ValueNotFound(string? message)
        {
            return Error.NotFound("record.not.found", $"record {message} not found");
        }
        
        public static Error Conflict(string? message)
        {
            return Error.Conflict("logic.conflict", $"{message}");
        }

        public static Error LengthIsInvalid(int lessThen, string? propertyName = null)
        {
            var text = propertyName ?? ""; 
            return Error.Validation("value.length.invalid",
                $"{text} length is invalid. Maximum length is {lessThen}.");
        }
    }
}