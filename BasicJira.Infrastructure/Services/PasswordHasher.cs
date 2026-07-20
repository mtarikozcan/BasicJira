using BasicJira.Application.Common.Interfaces;

namespace BasicJira.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);        // ipasswordhasher implemente ediliyor ve BCrypt kütüphanesi kullanılarak şifre hashleniyor.
    }

    public bool VerifyPassword(
        string password,              
        string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(
            password,
            passwordHash);
    }
}