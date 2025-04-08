using CSharpFunctionalExtensions;
using FileService.API;
using FileService.Endpoints;
using FileService.Jobs;
using Hangfire;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace FileService.Features;

public static class TestHangfire
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("hangfire-test", Handler);
        }
    }

    private static IResult Handler(CancellationToken cancellationToken = default)
    {
        var jobId = BackgroundJob.Schedule<ConfirmConsistencyJob>(j => j
            .Execute(Guid.NewGuid(), "key"), TimeSpan.FromSeconds(5));

        return CustomResponses.Ok(jobId);
    }
}