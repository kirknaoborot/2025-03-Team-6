FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Team-6.sln ./
COPY AuthService/Authorization.Api.csproj AuthService/
COPY Auth.Application/Auth.Application.csproj Auth.Application/
COPY Auth.Core/Auth.Core.csproj Auth.Core/
COPY Auth.DataAccess/Auth.DataAccess.csproj Auth.DataAccess/
COPY Auth.Domain/Auth.Domain.csproj Auth.Domain/
COPY Infrastructure.Shared/Infrastructure.Shared.csproj Infrastructure.Shared/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet build "AuthService/Authorization.Api.csproj" -c Release -o /app/build
RUN dotnet publish "AuthService/Authorization.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Authorization.Api.dll"]
