using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

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