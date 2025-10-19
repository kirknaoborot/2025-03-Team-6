# PowerShell script to start development environment
Write-Host "Starting development environment..." -ForegroundColor Green

# Start only infrastructure services
docker-compose -f docker-compose.dev.yml up -d

Write-Host "Infrastructure services started:" -ForegroundColor Yellow
Write-Host "- PostgreSQL: localhost:5432" -ForegroundColor Cyan
Write-Host "- RabbitMQ Management: http://localhost:15672 (admin/admin)" -ForegroundColor Cyan
Write-Host "- Seq Logging: http://localhost:5341" -ForegroundColor Cyan

Write-Host "`nNow you can run the .NET services locally:" -ForegroundColor Yellow
Write-Host "1. Start API Gateway: dotnet run --project ApiGateway" -ForegroundColor White
Write-Host "2. Start Auth Service: dotnet run --project AuthService" -ForegroundColor White
Write-Host "3. Start Conversation Service: dotnet run --project ConversationService.Api" -ForegroundColor White
Write-Host "4. Start Channel Settings: dotnet run --project ChannelSettings" -ForegroundColor White
Write-Host "5. Start Frontend: cd frontend && npm run dev" -ForegroundColor White
