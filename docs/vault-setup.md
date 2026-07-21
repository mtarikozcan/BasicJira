# Vault Development Setup

BasicJira API bilgisayarda doğrudan çalışır. Vault ise Docker container içinde, development modunda çalışır.

Vault dev mode kullandığı için container yeniden oluşturulduğunda aşağıdaki veriler silinir:

- AppRole auth yapılandırması
- Policy
- Role ID
- Secret ID
- Application secret'ları

## 1. Vault'u başlat

Proje kökünde:

```powershell
.\scripts\start-vault.ps1
```

Kontrol:

```powershell
docker ps --filter "name=basicjira-vault"
```

## 2. AppRole auth metodunu aç

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault auth enable approle
```

## 3. Policy dosyasını container'a kopyala

Proje kökünde bulunan `basicjira-policy.hcl` dosyasını kullan:

```powershell
docker cp `
  .\basicjira-policy.hcl `
  basicjira-vault:/tmp/basicjira-policy.hcl
```

Policy'yi Vault'a yükle:

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault policy write `
  basicjira-api-policy `
  /tmp/basicjira-policy.hcl
```

## 4. AppRole oluştur

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault write `
  auth/approle/role/basicjira-api `
  token_policies="basicjira-api-policy" `
  token_ttl="1h" `
  token_max_ttl="4h"
```

## 5. Role ID ve Secret ID al

Role ID:

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault read `
  -field=role_id `
  auth/approle/role/basicjira-api/role-id
```

Secret ID:

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault write `
  -field=secret_id `
  -f `
  auth/approle/role/basicjira-api/secret-id
```

Bu değerleri `BasicJira.Api` klasöründe User Secrets'a yaz:

```powershell
dotnet user-secrets set "Vault:RoleId" "ROLE_ID"
dotnet user-secrets set "Vault:SecretId" "SECRET_ID"
```

Kontrol:

```powershell
dotnet user-secrets list
```

User Secrets içinde yalnızca Vault bağlantısı için gereken gizli bilgiler bulunmalıdır:

```text
Vault:RoleId
Vault:SecretId
```

Vault adresi `appsettings.Development.json` içinden okunur.

## 6. Application secret'larını Vault'a yaz

Gerçek development değerlerini kendi ortamına göre doldur:

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault kv put secret/basicjira `
  ConnectionStrings__DefaultConnection="CONNECTION_STRING" `
  EmailSettings__Password="EMAIL_PASSWORD" `
  Jwt__SecretKey="JWT_SECRET_KEY"
```

Vault anahtarlarındaki çift alt çizgi, uygulama açılırken `:` karakterine dönüştürülür.

Örnek:

```text
Jwt__SecretKey
```

uygulama configuration içinde:

```text
Jwt:SecretKey
```

olarak kullanılır.

## 7. Secret'ları kontrol et

```powershell
docker exec `
  -e VAULT_ADDR=http://127.0.0.1:8200 `
  -e VAULT_TOKEN=basicjira-root-token `
  basicjira-vault `
  vault kv get secret/basicjira
```

Beklenen key'ler:

```text
ConnectionStrings__DefaultConnection
EmailSettings__Password
Jwt__SecretKey
```

## 8. Uygulamayı çalıştır

Visual Studio üzerinden `BasicJira.Api` projesini çalıştır.

Kontrol edilecekler:

- API açılıyor.
- Veritabanı bağlantısı kuruluyor.
- Login endpoint'i JWT oluşturuyor.
- Yetkilendirilmiş endpoint'ler Swagger üzerinden çalışıyor.

## Güvenlik notu

Aşağıdaki değerler Git'e eklenmemelidir:

- Role ID
- Secret ID
- JWT secret
- Mail parolası
- Connection string içindeki gerçek credential'lar
- Vault backup dosyaları