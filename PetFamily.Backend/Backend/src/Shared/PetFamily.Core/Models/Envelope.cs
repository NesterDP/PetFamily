using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Core.Models;

public record Envelope
{
    public object? Result { get; }

    public ErrorList? Errors { get; }

    public DateTime? TimeGenerated { get; }

    private Envelope(object? result, ErrorList? errors)
    {
        Result = result;
        if (errors != null)
            Errors = errors.ToList();
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result) =>
        new(result, null);

    public static Envelope Error(ErrorList errors) =>
        new(null, errors);
}