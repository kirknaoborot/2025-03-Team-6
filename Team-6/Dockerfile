# Multi-stage build for .NET applications
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Team-6.sln ./

# Copy all project files
COPY ApiGateway/ApiGateway.csproj ApiGateway/
COPY AuthService/Authorization.Api.csproj AuthService/
COPY Auth.Application/Auth.Application.csproj Auth.Application/
COPY Auth.Core/Auth.Core.csproj Auth.Core/
COPY Auth.DataAccess/Auth.DataAccess.csproj Auth.DataAccess/
COPY Auth.Domain/Auth.Domain.csproj Auth.Domain/
COPY ChannelSettings/ChannelSettings.csproj ChannelSettings/
COPY ChannelSettings.Application/ChannelSettings.Application.csproj ChannelSettings.Application/
COPY ChannelSettings.Core/ChannelSettings.Core.csproj ChannelSettings.Core/
COPY ChannelSettings.DataAccess/ChannelSettings.DataAccess.csproj ChannelSettings.DataAccess/
COPY ChannelSettings.Domain/ChannelSettings.Domain.csproj ChannelSettings.Domain/
COPY ConversationService.Api/ConversationService.Api.csproj ConversationService.Api/
COPY ConversationService.Application/ConversationService.Application.csproj ConversationService.Application/
COPY ConversationService.Domain/ConversationService.Domain.csproj ConversationService.Domain/
COPY ConversationService.Infrastructure/ConversationService.Infrastructure.csproj ConversationService.Infrastructure/
COPY ConversationDistributed/ConversationDistributed.csproj ConversationDistributed/
COPY Infrastructure.Shared/Infrastructure.Shared.csproj Infrastructure.Shared/
COPY MessageHubService.Application/MessageHubService.Application.csproj MessageHubService.Application/
COPY MessageHubService.Domain/MessageHubService.Domain.csproj MessageHubService.Domain/
COPY MessageHubService.Infrastructure/MessageHubService.Infrastructure.csproj MessageHubService.Infrastructure/
COPY OnlineStatusService/OnlineStatusService/OnlineStatusService.csproj OnlineStatusService/OnlineStatusService/
COPY OperatorStatusService/OperatorStatusService.csproj OperatorStatusService/
COPY OperatorStatusService.Application/OperatorStatusService.Application.csproj OperatorStatusService.Application/
COPY OperatorStatusService.Domain/OperatorStatusService.Domain.csproj OperatorStatusService.Domain/
COPY OperatorStatusService.Infrastructure/OperatorStatusService.Infrastructure.csproj OperatorStatusService.Infrastructure/
COPY OrchestratService.Application/OrchestratService.Application.csproj OrchestratService.Application/
COPY ProfileService.Api/ProfileService.Api.csproj ProfileService.Api/
COPY ProfileService.Application/ProfileService.Application.csproj ProfileService.Application/
COPY ProfileService.Domain/ProfileService.Domain.csproj ProfileService.Domain/
COPY ProfileService.Infrastructure/ProfileService.Infrastructure.csproj ProfileService.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
