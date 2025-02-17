using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.API.Response;

public record Envelope
{
    public object? Result { get; }
    public List<ResponseError> Errors { get; }
    public DateTime? TimeGenerated { get; }

    private Envelope(object? result, IEnumerable<ResponseError> errors)
    {
        Result = result;
        Errors = errors.Count() == 0 ? [new ResponseError(null, null, null)] : errors.ToList();
        TimeGenerated = DateTime.Now;
    }

    public static Envelope Ok(object? result) =>
        new (result, []);

    public static Envelope Error(IEnumerable<ResponseError> errors) =>
        new (null, errors);
}