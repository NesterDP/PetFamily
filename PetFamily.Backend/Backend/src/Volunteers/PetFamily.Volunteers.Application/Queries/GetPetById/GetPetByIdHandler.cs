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
        
        result.Photos.Select(p => p.Id).ToList();
        //var request = new GetFilesPresignedUrlsRequest(result.Photos.Select(p => p.Id).ToList());
        
        var test1 = Guid.Parse("ed139f4c-b13c-49af-bbb1-fff9e3648d4d");
        var test2 = Guid.Parse("280beb27-cc2d-417b-86f3-619b9f329bb6");
        var request = new GetFilesPresignedUrlsRequest([test1, test2]);
        
        var photosData = await _httpClient.GetFilesPresignedUrls(request, cancellationToken);

        foreach (var photoDto in result.Photos)
        {
            photoDto.Url = photosData.Value.FilesInfos.FirstOrDefault(d => d.Id == photoDto.Id)?.Url;
        }
        
        return result;
    }
}