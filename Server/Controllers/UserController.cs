using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    // yourdomain.com/api/user

    public UserController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    // yourdomain.com/api/user/register
    [Route("register")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        string username = user.EmailAddress;
        string password = user.Password;

        IdentityUser identityUser = new IdentityUser
        {
            Email = username,
            UserName = username
        };

        IdentityResult userIdentityResult = await _userManager.CreateAsync(identityUser, password);
        IdentityResult roleIdentityResult = await _userManager.AddToRoleAsync(identityUser, "Administrator");

        if (userIdentityResult.Succeeded == true && roleIdentityResult.Succeeded == true)
        {
            return Ok(new { userIdentityResult.Succeeded });
        }
        else
        {
            string errorsToReturn = "Register failed with the following errors";

            foreach (var error in userIdentityResult.Errors)
            {
                errorsToReturn += Environment.NewLine;
                errorsToReturn += $"Error code: {error.Code} - {error.Description}";
            }

            return StatusCode(StatusCodes.Status500InternalServerError, errorsToReturn);
        }
    }

    [Route("signin")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] User user)
    {
        string username = user.EmailAddress;
        string password = user.Password;

        Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);

        if (signInResult.Succeeded == true)
        {
            IdentityUser identityUser = await _userManager.FindByNameAsync(username);
            string JSONWebTokenAsString = await GenerateJSONWebToken(identityUser);
            return Ok(JSONWebTokenAsString);
        }
        else
        {
            return Unauthorized(user);
        }
    }

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<string> GenerateJSONWebToken(IdentityUser identityUser)
    {
        SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        SigningCredentials credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        // Claim = who is the person trying to sign in claming to be?
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, identityUser.Id)
        };

        IList<string> roleNames = await _userManager.GetRolesAsync(identityUser);
        claims.AddRange(roleNames.Select(roleName => new Claim(ClaimsIdentity.DefaultRoleClaimType, roleName)));

        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
        (
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            null,
            expires: DateTime.UtcNow.AddDays(28),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}
