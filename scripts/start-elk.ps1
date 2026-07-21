$ErrorActionPreference = "Stop"

$networkName = "basicjira-network"
$elasticContainer = "basicjira-elasticsearch"
$kibanaContainer = "basicjira-kibana"
$elasticVersion = "8.19.17"

Write-Host "Eski Elasticsearch ve Kibana container'ları kontrol ediliyor..."

docker rm -f $kibanaContainer 2>$null | Out-Null
docker rm -f $elasticContainer 2>$null | Out-Null

$networkExists = docker network ls `
    --filter "name=^$networkName$" `
    --format "{{.Name}}"

if ($networkExists -ne $networkName) {
    Write-Host "Docker network oluşturuluyor..."
    docker network create $networkName | Out-Null
}

Write-Host "Elasticsearch başlatılıyor..."

docker run -d `
    --name $elasticContainer `
    --network $networkName `
    -p 127.0.0.1:9200:9200 `
    -m 1GB `
    -e "discovery.type=single-node" `
    -e "xpack.security.enabled=false" `
    -e "xpack.security.enrollment.enabled=false" `
    docker.elastic.co/elasticsearch/elasticsearch:$elasticVersion

Write-Host "Elasticsearch hazır olana kadar bekleniyor..."

$maxAttempts = 60

for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
    try {
        $response = Invoke-RestMethod `
            -Uri "http://localhost:9200" `
            -Method Get `
            -TimeoutSec 2

        if ($response.tagline -eq "You Know, for Search") {
            Write-Host "Elasticsearch hazır."
            break
        }
    }
    catch {
        if ($attempt -eq $maxAttempts) {
            Write-Error "Elasticsearch zamanında hazır olmadı. 'docker logs $elasticContainer' komutunu çalıştırın."
        }

        Start-Sleep -Seconds 2
    }
}

Write-Host "Kibana başlatılıyor..."

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
Write-Host "Kibana'nın tamamen açılması yaklaşık 30-90 saniye sürebilir."