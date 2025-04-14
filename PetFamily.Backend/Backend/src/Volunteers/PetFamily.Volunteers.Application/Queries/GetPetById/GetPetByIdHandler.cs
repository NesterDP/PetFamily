using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Pet;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Application.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandler<Result<PetDto, ErrorList>, GetPetByIdQuery>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileService _fileService;

    public GetPetByIdHandler(IReadDbContext readDbContext, IFileService fileService)
    {
        _readDbContext = readDbContext;
        _fileService = fileService;
    }
    
    public async Task<Result<PetDto, ErrorList>> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _readDbContext.Pets
            .FirstOrDefaultAsync(v => v.Id == query.Id && v.IsDeleted == false, cancellationToken);

        if (result is null)
            return Errors.General.ValueNotFound(query.Id).ToErrorList();
       
        result.Photos = result.Photos.OrderByDescending(p => p.Main).ToArray();
        
        var request = new GetFilesPresignedUrlsRequest(result.Photos.Select(p => p.Id).ToList());
        var photosData = await _fileService.GetFilesPresignedUrls(request, cancellationToken);
        if (photosData.IsFailure)
            return Errors.General.Failure(photosData.Error).ToErrorList();

        foreach (var photoDto in result.Photos)
            photoDto.Url = photosData.Value.FilesInfos.FirstOrDefault(d => d.Id == photoDto.Id)!.Url;
        
        return result;
    }
}