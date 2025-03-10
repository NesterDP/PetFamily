namespace PetFamily.Application.Abstractions;

public interface IQueryHandler<TResponse, TQuery> where TQuery : IQuery
{
    public Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}