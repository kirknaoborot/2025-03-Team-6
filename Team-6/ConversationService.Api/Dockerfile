FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Team-6.sln ./
COPY ConversationService.Api/ConversationService.Api.csproj ConversationService.Api/
COPY ConversationService.Application/ConversationService.Application.csproj ConversationService.Application/
COPY ConversationService.Domain/ConversationService.Domain.csproj ConversationService.Domain/
COPY ConversationService.Infrastructure/ConversationService.Infrastructure.csproj ConversationService.Infrastructure/
COPY Infrastructure.Shared/Infrastructure.Shared.csproj Infrastructure.Shared/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet build "ConversationService.Api/ConversationService.Api.csproj" -c Release -o /app/build
RUN dotnet publish "ConversationService.Api/ConversationService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConversationService.Api.dll"]
