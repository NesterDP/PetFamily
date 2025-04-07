using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Species.Domain.Entities;

public class Species : Entity<SpeciesId>
{
    public Name Name { get; private set; }

    private readonly List<Breed> _breeds = [];

    public IReadOnlyList<Breed> Breeds => _breeds;

    // ef core
    public Species() { }
    
    public Species(SpeciesId id, Name name) : base(id)
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
    
    public Result<Guid, Error> RemoveBreedById(BreedId breedId)
    {
        var breed = _breeds.FirstOrDefault(p => p.Id.Value == breedId.Value);
        if (breed == null)
            return Errors.General.ValueNotFound(breedId.Value);
        
        _breeds.Remove(breed);
        return breedId.Value;
    }
}