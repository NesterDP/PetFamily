using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Structs;
using PetFamily.Volunteers.Infrastructure.DbContexts;

namespace PetFamily.Volunteers.Infrastructure.Services;

public class DeleteExpiredEntitiesService 
{
    private readonly WriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExpiredEntitiesService(
        WriteDbContext dbContext,
        [FromKeyedServices(UnitOfWorkSelector.Volunteers)] IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Process(int lifetimeAfterDeletion, CancellationToken cancellationToken)
    {
        var volunteers = await _dbContext.Volunteers.ToListAsync(cancellationToken);

        foreach (var volunteer in volunteers)
        {
            if (volunteer.DeletionDate != null &&
                DateTime.UtcNow >= volunteer.DeletionDate.Value.AddDays(lifetimeAfterDeletion))
                //DateTime.UtcNow >= volunteer.DeletionDate.Value.AddMinutes(lifetimeAfterDeletion))
            {
                _dbContext.Volunteers.Remove(volunteer);
                continue;
            }
            
            volunteer.DeleteExpiredPets(lifetimeAfterDeletion);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}