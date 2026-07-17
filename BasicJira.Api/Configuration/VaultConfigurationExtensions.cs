using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.Core;
using VaultSharp.V1.AuthMethods.AppRole;

namespace BasicJira.Api.Configuration;

public static class VaultConfigurationExtensions
{
    public static async Task AddVaultSecretsAsync(
        this ConfigurationManager configuration)
    {
        var address = configuration["Vault:Address"];
        var roleId = configuration["Vault:RoleId"];
        var secretId = configuration["Vault:SecretId"];
        var mountPoint = configuration["Vault:MountPoint"] ?? "secret";
        var path = configuration["Vault:Path"] ?? "basicjira";

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidOperationException(
                "Vault:Address ayarı bulunamadı.");
        }

        if (string.IsNullOrWhiteSpace(roleId) ||
            string.IsNullOrWhiteSpace(secretId))
        {
            throw new InvalidOperationException(
                "Vault AppRole bilgileri eksik. User Secrets içindeki " +
                "Vault:RoleId ve Vault:SecretId değerlerini kontrol edin.");
        }

        var authMethod = new AppRoleAuthMethodInfo(roleId, secretId); // 

        var clientSettings = new VaultClientSettings(
            address,
            authMethod);

        var client = new VaultClient(clientSettings);

        try
        {
            var secret =
                await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                    path: path,
                    mountPoint: mountPoint);

            if (secret.Data.Data.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Vault secret boş: {mountPoint}/{path}");
            }

            var values = secret.Data.Data.ToDictionary(
                item => item.Key.Replace("__", ":"),
                item => item.Value?.ToString());

            configuration.AddInMemoryCollection(values);
        }
        catch (VaultApiException ex)
        {
            throw new InvalidOperationException(
                $"Vault secret okunamadı: {mountPoint}/{path}. " +
                "AppRole, policy, RoleId, SecretId ve secret yolunu kontrol edin.",
                ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                $"Vault sunucusuna bağlanılamadı: {address}.",
                ex);
        }
    }
}