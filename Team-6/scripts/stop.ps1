# PowerShell script to stop all services
Write-Host "Stopping all services..." -ForegroundColor Yellow

# Stop production environment
docker-compose down

# Stop development environment
docker-compose -f docker-compose.dev.yml down

Write-Host "All services stopped." -ForegroundColor Green
