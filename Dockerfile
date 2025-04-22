# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Projenin tümünü kopyala
COPY . .

# .csproj yolunu doğru ver
RUN dotnet publish FuarYonetimSistemi/FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj -c Release -o /app/out

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FuarYonetimSistemi.API.dll"]
