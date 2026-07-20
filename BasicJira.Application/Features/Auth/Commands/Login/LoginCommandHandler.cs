using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "E-posta veya şifre hatalı.");
        }

        return _jwtTokenGenerator.GenerateToken(user);
    }
}