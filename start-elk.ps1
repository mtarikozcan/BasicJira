@'
$ErrorActionPreference = "Stop"

$networkName = "basicjira-network"
$elasticContainer = "basicjira-elasticsearch"
$kibanaContainer = "basicjira-kibana"
$elasticVersion = "8.19.17"

Write-Host "Eski Elasticsearch ve Kibana container'lari kontrol ediliyor..."

$existingKibana = docker ps -a `
    --filter "name=^/$kibanaContainer$" `
    --format "{{.Names}}"

if ($existingKibana -eq $kibanaContainer) {
    Write-Host "Eski Kibana container kaldiriliyor..."
    docker rm -f $kibanaContainer | Out-Null
}

$existingElastic = docker ps -a `
    --filter "name=^/$elasticContainer$" `
    --format "{{.Names}}"

if ($existingElastic -eq $elasticContainer) {
    Write-Host "Eski Elasticsearch container kaldiriliyor..."
    docker rm -f $elasticContainer | Out-Null
}

$networkExists = docker network ls `
    --filter "name=^$networkName$" `
    --format "{{.Name}}"

if ($networkExists -ne $networkName) {
    Write-Host "Docker network olusturuluyor..."
    docker network create $networkName | Out-Null
}

Write-Host "Elasticsearch baslatiliyor..."

docker run -d `
    --name $elasticContainer `
    --network $networkName `
    -p 127.0.0.1:9200:9200 `
    -m 1GB `
    -e "discovery.type=single-node" `
    -e "xpack.security.enabled=false" `
    -e "xpack.security.enrollment.enabled=false" `
    docker.elastic.co/elasticsearch/elasticsearch:$elasticVersion

Write-Host "Elasticsearch hazir olana kadar bekleniyor..."

$maxAttempts = 60
$elasticReady = $false

for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
    try {
        $response = Invoke-RestMethod `
            -Uri "http://localhost:9200" `
            -Method Get `
            -TimeoutSec 2

        if ($response.tagline -eq "You Know, for Search") {
            $elasticReady = $true
            Write-Host "Elasticsearch hazir."
            break
        }
    }
    catch {
        Start-Sleep -Seconds 2
    }
}

if (-not $elasticReady) {
    throw "Elasticsearch zamaninda hazir olmadi. 'docker logs $elasticContainer' komutunu calistirin."
}

Write-Host "Kibana baslatiliyor..."

docker run -d `
    --name $kibanaContainer `
    --network $networkName `
    -p 127.0.0.1:5601:5601 `
    -e "ELASTICSEARCH_HOSTS=http://${elasticContainer}:9200" `
    docker.elastic.co/kibana/kibana:$elasticVersion

Write-Host ""
Write-Host "Elasticsearch: http://localhost:9200"
Write-Host "Kibana:        http://localhost:5601"
Write-Host ""
Write-Host "Kibana'nin tamamen acilmasi 30-90 saniye surebilir."
'@ | Set-Content .\start-elk.ps1 -Encoding UTF8