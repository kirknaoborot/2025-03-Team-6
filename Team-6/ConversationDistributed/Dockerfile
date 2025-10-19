FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Team-6.sln ./
COPY ConversationDistributed/ConversationDistributed.csproj ConversationDistributed/
COPY Infrastructure.Shared/Infrastructure.Shared.csproj Infrastructure.Shared/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build and publish
RUN dotnet build "ConversationDistributed/ConversationDistributed.csproj" -c Release -o /app/build
RUN dotnet publish "ConversationDistributed/ConversationDistributed.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ConversationDistributed.dll"]
