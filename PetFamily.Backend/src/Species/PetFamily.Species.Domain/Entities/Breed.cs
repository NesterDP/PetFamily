using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Species.Domain.Entities;

public class Breed : Entity<BreedId>
{
    public Name Name { get; private set; }
    
    // ef core
    public Breed(BreedId id) : base(id) { }

    public Breed(BreedId id, Name name) : base(id)
    {
        Name = name;
    }
}