# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Tüm projeleri kopyala
COPY . .

# Ana projeyi restore et
WORKDIR /src/FuarYonetimSistemi
RUN dotnet restore

# Publish
RUN dotnet publish -c Release -o /app/publish

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FuarYonetimSistemi.dll"]
