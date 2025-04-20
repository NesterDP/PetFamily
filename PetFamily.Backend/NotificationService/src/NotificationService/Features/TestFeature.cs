using NotificationService.API.Endpoints;

namespace NotificationService.Features;

public static class TestFeature
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("test", Handler);
        }
    }

    private static async Task<IResult> Handler()
    {
        var response = "response";

        return Results.Ok(response);
    }
}