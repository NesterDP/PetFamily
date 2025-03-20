using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Accounts.Application.Commands.Register;
using PetFamily.Accounts.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Authorization;

namespace PetFamily.Accounts.Presentation;

public class AccountsController : ApplicationController
{
    [Permission(Permissions.Volunteers.CREATE_VOLUNTEER)]
    //[Authorize(Policy = "volunteers.create")]
    [HttpPost("create")]
    public async Task<IActionResult> TestAdmin()
    {
        return Ok("All is ok");
    }
    
   // [Permission("volunteers.update")]
    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> TestUser()
    {
        return Ok("All is ok");
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
        return result.IsFailure ? result.Error.ToResponse() : result.ToResponse();
    }
}