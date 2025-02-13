using CSharpFunctionalExtensions;
using PetFamily.Domain.PetContext.Entities;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Species : Shared.Entity<SpeciesId>
{
    public Name Name { get; private set; }

    private readonly List<Breed> _breeds = [];

    public IReadOnlyCollection<Breed> Breeds => _breeds;

    // ef core
    public Species(SpeciesId id) : base(id) { }

    public Species(SpeciesId id, Name name, List<Breed> breeds) : base(id)
    {
        Name = name;
    }
}