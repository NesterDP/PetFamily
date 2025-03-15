using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Framework;

namespace PetFamily.Accounts.Presentation;

public class AccountsController : ApplicationController
{
    [HttpPost("jwt")]
    public ActionResult LoginToMy(CancellationToken cancellationToken)
    {
        var user = HttpContext.User;
        var claims = new[]
        {
            //new Claim(JwtRegisteredClaimNames.Sub, "userId"),
            new Claim("Sasha", "Nesterenko")
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "test",
            audience: "test",
            claims: claims,
            signingCredentials: creds);
        
        var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(stringToken);
    }
}