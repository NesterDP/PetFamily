using CSharpFunctionalExtensions;
using PetFamily.Core.SharedVO;
using PetFamily.Species.Domain.ValueObjects.BreedVO;

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