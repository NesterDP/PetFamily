using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Abstractions;

public interface ICommandHandler<TResponse, TCommand> where TCommand : ICommand
{
    public Task<Result<TResponse, ErrorList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public Task<UnitResult<ErrorList>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}