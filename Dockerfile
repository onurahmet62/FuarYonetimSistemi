# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj", "FuarYonetimSistemi.API/"]
COPY ["FuarYonetimSistemi.Application/FuarYonetimSistemi.Application.csproj", "FuarYonetimSistemi.Application/"]
COPY ["FuarYonetimSistemi.Domain/FuarYonetimSistemi.Domain.csproj", "FuarYonetimSistemi.Domain/"]
COPY ["FuarYonetimSistemi.Infrastructure/FuarYonetimSistemi.Infrastructure.csproj", "FuarYonetimSistemi.Infrastructure/"]
COPY ["FuarYonetimSistemi.Persistence/FuarYonetimSistemi.Persistence.csproj", "FuarYonetimSistemi.Persistence/"]

# Restore the dependencies
RUN dotnet restore "FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj"

# Copy all files and build the project
COPY . .
WORKDIR "/src/FuarYonetimSistemi.API"
RUN dotnet build "FuarYonetimSistemi.API.csproj" -c Release -o /app/build

# Publish the application to the /app/publish directory
FROM build AS publish
RUN dotnet publish "FuarYonetimSistemi.API.csproj" -c Release -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the published files from the publish stage
COPY --from=publish /app/publish .

# Set the entry point to run the API application
ENTRYPOINT ["dotnet", "FuarYonetimSistemi.API.dll"]
