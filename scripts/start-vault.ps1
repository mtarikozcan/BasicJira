$ErrorActionPreference = "Stop"

$containerName = "basicjira-vault"
$vaultImage = "hashicorp/vault:2.0.3"

$existingContainer = docker ps -a `
    --filter "name=^/$containerName$" `
    --format "{{.Names}}"

if ($existingContainer -eq $containerName) {
    Write-Host "Eski Vault container kaldırılıyor..."
    docker rm -f $containerName | Out-Null
}

Write-Host "Vault dev container başlatılıyor..."

docker run -d `
    --name $containerName `
    --cap-add=IPC_LOCK `
    -p 8200:8200 `
    -e VAULT_DEV_ROOT_TOKEN_ID=basicjira-root-token `
    -e VAULT_DEV_LISTEN_ADDRESS=0.0.0.0:8200 `
    --restart unless-stopped `
    $vaultImage `
    server -dev

Write-Host ""
Write-Host "Vault başlatıldı: http://localhost:8200"
Write-Host "Dev mode kullanıldığı için container yeniden oluşturulursa Vault verileri silinir."
Write-Host "docs/vault.md içindeki AppRole ve secret kurulum adımlarını uygulayın."

// run command - > .\scripts\start-vault.ps1