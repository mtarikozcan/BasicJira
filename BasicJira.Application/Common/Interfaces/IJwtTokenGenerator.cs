using BasicJira.Domain.Entities;

namespace BasicJira.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user);
}