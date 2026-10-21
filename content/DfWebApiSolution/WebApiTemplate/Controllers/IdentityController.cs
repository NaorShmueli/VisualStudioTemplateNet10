using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTemplate.Controllers;

[ApiController]
[Authorize]
public abstract class IdentityController : ControllerBase
{
    protected string UserId => TryGetUserId()
        ?? throw new UnauthorizedAccessException("The authenticated token has no subject claim.");

    protected string? TryGetUserId()
    {
        return User.FindFirstValue("sub")
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
