# PowerShell script to start production environment
Write-Host "Building and starting production environment..." -ForegroundColor Green

# Build and start all services
docker-compose up --build -d

Write-Host "Production environment started:" -ForegroundColor Yellow
Write-Host "- Frontend: http://localhost:3000" -ForegroundColor Cyan
Write-Host "- API Gateway: http://localhost:5003" -ForegroundColor Cyan
Write-Host "- Auth Service: http://localhost:56468" -ForegroundColor Cyan
Write-Host "- Conversation Service: http://localhost:55822" -ForegroundColor Cyan
Write-Host "- Channel Settings: http://localhost:7220" -ForegroundColor Cyan
Write-Host "- RabbitMQ Management: http://localhost:15672 (admin/admin)" -ForegroundColor Cyan
Write-Host "- Seq Logging: http://localhost:5341" -ForegroundColor Cyan

Write-Host "`nTo view logs: docker-compose logs -f [service-name]" -ForegroundColor Yellow
Write-Host "To stop: docker-compose down" -ForegroundColor Yellow
