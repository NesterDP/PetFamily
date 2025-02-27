using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.BreedVO;
using PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Species : Entity<SpeciesId>
{
    public Name Name { get; private set; }

    private readonly List<Breed> _breeds = [];

    public IReadOnlyList<Breed> Breeds => _breeds;

    // ef core
    public Species(SpeciesId id) : base(id) { }
    


    public Species(SpeciesId id, Name name, List<Breed> breeds) : base(id)
    {
        Name = name;
    }
    
    public UnitResult<Error> AddBreed(Breed breed)
    {
        _breeds.Add(breed);
        return Result.Success<Error>();
    }
    public Result<Breed, Error> GetBreedByName(string breedName)
    {
        var breed = _breeds.FirstOrDefault(p => p.Name.Value == breedName);
        if (breed == null)
            return Errors.General.ValueNotFound();
        return breed;
    }
    
    public Result<Breed, Error> GetBreedById(BreedId breedId)
    {
        var breed = _breeds.FirstOrDefault(p => p.Id.Value == breedId.Value);
        if (breed == null)
            return Errors.General.ValueNotFound(breedId.Value);
        return breed;
    }
}