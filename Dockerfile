# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Proje dosyalarını doğru kopyala
COPY . .

# .csproj yolunun doğru olduğuna emin olalım
RUN ls /src/FuarYonetimSistemi/FuarYonetimSistemi.API

# .csproj dosyasını publish et
RUN dotnet publish FuarYonetimSistemi/FuarYonetimSistemi.API/FuarYonetimSistemi.API.csproj -c Release -o /app/out

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000

ENTRYPOINT ["dotnet", "FuarYonetimSistemi.API.dll"]
