using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;

namespace PetFamily.Volunteers.Application.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandler<PetDto, GetPetByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly FileHttpClient _httpClient;

    public GetPetByIdHandler(IReadDbContext readDbContext, FileHttpClient httpClient)
    {
        _readDbContext = readDbContext;
        _httpClient = httpClient;
    }

    public async Task<PetDto> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _readDbContext.Pets
            .FirstOrDefaultAsync(v => v.Id == query.Id && v.IsDeleted == false, cancellationToken);
        
        if (result != null)
            result.Photos = result.Photos.OrderByDescending(p => p.Main).ToArray();
        
        // TODO: здесь должен быть вызов FileService, который создаст URL для всех фото питомца
        var photosIds = result.Photos.Select(p => p.Id).ToList();
        var request = new GetFilesPresignedUrlsRequest(photosIds);
        var photosData = await _httpClient.GetFilesPresignedUrls()
        
        //var fullResult = result.Photos.Join(photosData.Value, photo => photo.Id, data => data.FilesInfos.)
        
        return result;
    }
}