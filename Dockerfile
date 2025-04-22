# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Proje dosyalarını kopyala ve publish et
COPY . . 
RUN dotnet publish FuarYonetimSistemi.API.csproj -c Release -o out

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Port belirt (Kestrel default: 5000)
EXPOSE 5000

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "FuarYonetimSistemi.API.dll"]
