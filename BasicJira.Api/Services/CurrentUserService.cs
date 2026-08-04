using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BasicJira.Application.Common.Interfaces;

namespace BasicJira.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? CurrentUser =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        CurrentUser?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var userIdClaim =
                CurrentUser?.FindFirstValue(
                    JwtRegisteredClaimNames.Sub)
                ?? CurrentUser?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                userIdClaim,
                out var userId)
                    ? userId
                    : null;
        }
    }

    public string? Email =>
        CurrentUser?.FindFirstValue(
            JwtRegisteredClaimNames.Email)
        ?? CurrentUser?.FindFirstValue(
            ClaimTypes.Email);

    public string? Role =>
        CurrentUser?.FindFirstValue(
            ClaimTypes.Role);
}