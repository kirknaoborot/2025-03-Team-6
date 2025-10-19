FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Team-6.sln ./
COPY ChannelSettings/ChannelSettings.csproj ChannelSettings/
COPY ChannelSettings.Application/ChannelSettings.Application.csproj ChannelSettings.Application/
COPY ChannelSettings.Core/ChannelSettings.Core.csproj ChannelSettings.Core/
COPY ChannelSettings.DataAccess/ChannelSettings.DataAccess.csproj ChannelSettings.DataAccess/
COPY ChannelSettings.Domain/ChannelSettings.Domain.csproj ChannelSettings.Domain/
COPY Infrastructure.Shared/Infrastructure.Shared.csproj Infrastructure.Shared/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet build "ChannelSettings/ChannelSettings.csproj" -c Release -o /app/build
RUN dotnet publish "ChannelSettings/ChannelSettings.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChannelSettings.dll"]
