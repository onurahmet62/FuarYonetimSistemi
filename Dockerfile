# Base image (dotnet sdk)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image (dotnet sdk)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj", "FuarYonetimSistemi.API/"]
COPY ["FuarYonetimSistemi.Application/FuarYonetimSistemi.Application.csproj", "FuarYonetimSistemi.Application/"]
COPY ["FuarYonetimSistemi.Domain/FuarYonetimSistemi.Domain.csproj", "FuarYonetimSistemi.Domain/"]
COPY ["FuarYonetimSistemi.Infrastructure/FuarYonetimSistemi.Infrastructure.csproj", "FuarYonetimSistemi.Infrastructure/"]
COPY ["FuarYonetimSistemi.Persistence/FuarYonetimSistemi.Persistence.csproj", "FuarYonetimSistemi.Persistence/"]
RUN dotnet restore "FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj"

COPY . .
WORKDIR "/src/FuarYonetimSistemi.API"
RUN dotnet build "FuarYonetimSistemi.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FuarYonetimSistemi.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FuarYonetimSistemi.API.dll"]
