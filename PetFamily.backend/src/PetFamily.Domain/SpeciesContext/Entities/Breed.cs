using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Breed : Entity
{
    public BreedId Id { get; private set; }
    public Name Name { get; private set; }

    public Breed(BreedId id, Name name)
    {
        Id = id;
        Name = name;
    }
}