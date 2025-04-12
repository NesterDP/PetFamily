using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Accounts.Application.Commands.RefreshTokens;
using PetFamily.Accounts.Application.Commands.Register;
using PetFamily.Accounts.Application.Commands.StartUploadAvatar;
using PetFamily.Accounts.Application.Queries.GetUserById;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Accounts.Presentation.Accounts.Requests;
using PetFamily.Framework;

namespace PetFamily.Accounts.Presentation.Accounts;

public class AccountsController : ApplicationController
{
    //[Permission( "accounts.GetUserInfoById")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserInfoById(
        [FromRoute] Guid id,
        [FromServices] GetUserInfoByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserInfoByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        [FromServices] LoginUserHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        HttpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken.ToString());
        //return result.ToResponse();
        return Ok(result.Value.AccessToken);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens(
        [FromBody] RefreshTokenRequest request,
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokensCommand(request.AccessToken, request.RefreshToken);
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        HttpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken.ToString());
        //HttpContext.Response.Cookies.Append("accessToken", result.Value.AccessToken);
        //return result.ToResponse();
        //return Ok();
        return Ok(result.Value.AccessToken);
    }

    [Authorize]
    [HttpPost("avatar-start")]
    public async Task<IActionResult> StartUploadAvatar(
        [FromBody] StartUploadAvatarRequest request,
        [FromServices] StartUploadAvatarHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
    
    [Authorize]
    [HttpPost("avatar-complete")]
    public async Task<IActionResult> CompleteUploadAvatar(
        [FromBody] CompleteUploadAvatarRequest request,
        [FromServices] CompleteUploadAvatarHandler handler,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(GetUserId().Value);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}