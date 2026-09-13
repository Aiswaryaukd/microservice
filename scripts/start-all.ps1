param(
    [int]$ProductApiPort = 5033,
    [int]$GatewayPort = 5083,
    [int]$BlazorPort = 5062
)

function Stop-ByPort {
    param([int]$Port)
    $conns = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    if ($conns) {
        $pids = $conns.OwningProcess | Select-Object -Unique
        foreach ($processId in $pids) {
            try { Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue } catch {}
        }
    }
}

Write-Output "Stopping any processes on ports $ProductApiPort, $GatewayPort, $BlazorPort"
Stop-ByPort -Port $ProductApiPort
Stop-ByPort -Port $GatewayPort
Stop-ByPort -Port $BlazorPort

# Ensure Development so Swagger, seed data, and HTTP-only local runs work
$env:ASPNETCORE_ENVIRONMENT = 'Development'

Write-Output "Starting Product.Api on port $ProductApiPort"
Start-Process -FilePath dotnet -ArgumentList "run --project .\Product.Api\Product.Api.csproj --no-launch-profile --urls http://localhost:$ProductApiPort" -WorkingDirectory (Get-Location) -WindowStyle Hidden

# wait for health
for ($i=0;$i -lt 30;$i++) {
    try {
        $r = Invoke-RestMethod -Uri "http://localhost:$ProductApiPort/api/product" -Method Get -ErrorAction Stop
        if ($r) { break }
    } catch { Start-Sleep -Seconds 1 }
}

Write-Output "Starting APIGateway on port $GatewayPort"
Start-Process -FilePath dotnet -ArgumentList "run --project .\APIGateway\APIGateway.csproj --no-launch-profile --urls http://localhost:$GatewayPort" -WorkingDirectory (Get-Location) -WindowStyle Hidden

# wait for gateway
for ($i=0;$i -lt 20;$i++) {
    try { Invoke-RestMethod -Uri "http://localhost:$GatewayPort/" -Method Get -ErrorAction Stop; break } catch { Start-Sleep -Seconds 1 }
}

Write-Output "Starting BlazorApp on port $BlazorPort"
Start-Process -FilePath dotnet -ArgumentList "run --project .\BlazorApp\BlazorApp.csproj --no-launch-profile --urls http://localhost:$BlazorPort" -WorkingDirectory (Get-Location) -WindowStyle Hidden

Write-Output "All services started (or are starting). Open http://localhost:$BlazorPort for UI."
