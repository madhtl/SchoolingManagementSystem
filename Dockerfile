# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore dependencies
COPY ["YinStudio.csproj", "./"]
RUN dotnet restore "YinStudio.csproj"

# Copy the rest of the code
COPY . .
RUN dotnet build "YinStudio.csproj" -c Release -o /app/build

# Publish app
FROM build AS publish
RUN dotnet publish "YinStudio.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YinStudio.dll"]
