using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BasicJira.Infrastructure.Persistence;

public sealed class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string connectionString = GetConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        // Önce environment variable kontrol edilir.
        string? environmentConnectionString =
            Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection");

        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            return environmentConnectionString;
        }

        string apiProjectPath = ResolveApiProjectPath();

        // Development dosyası önce okunur.
        string developmentSettingsPath = Path.Combine(
            apiProjectPath,
            "appsettings.Development.json");

        string? connectionString =
            ReadConnectionString(developmentSettingsPath);

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        // Development dosyasında yoksa ana appsettings okunur.
        string appSettingsPath = Path.Combine(
            apiProjectPath,
            "appsettings.json");

        connectionString = ReadConnectionString(appSettingsPath);

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        throw new InvalidOperationException(
            "ConnectionStrings:DefaultConnection bulunamadı. " +
            "Environment variable, appsettings.Development.json " +
            "ve appsettings.json kontrol edildi.");
    }

    private static string? ReadConnectionString(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        string jsonContent = File.ReadAllText(filePath);

        using JsonDocument document = JsonDocument.Parse(
            jsonContent,
            new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            });

        JsonElement root = document.RootElement;

        if (!root.TryGetProperty(
                "ConnectionStrings",
                out JsonElement connectionStrings))
        {
            return null;
        }

        if (!connectionStrings.TryGetProperty(
                "DefaultConnection",
                out JsonElement defaultConnection))
        {
            return null;
        }

        return defaultConnection.GetString();
    }

    private static string ResolveApiProjectPath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        // Komut BasicJira.Api klasöründen çalıştırılmış olabilir.
        string currentAppSettingsPath = Path.Combine(
            currentDirectory,
            "appsettings.json");

        if (File.Exists(currentAppSettingsPath))
        {
            return currentDirectory;
        }

        // Komut solution root klasöründen çalıştırılmış olabilir.
        string apiProjectPath = Path.Combine(
            currentDirectory,
            "BasicJira.Api");

        string apiAppSettingsPath = Path.Combine(
            apiProjectPath,
            "appsettings.json");

        if (File.Exists(apiAppSettingsPath))
        {
            return apiProjectPath;
        }

        throw new DirectoryNotFoundException(
            "BasicJira.Api klasörü veya appsettings.json bulunamadı. " +
            $"Çalışma dizini: {currentDirectory}");
    }
}