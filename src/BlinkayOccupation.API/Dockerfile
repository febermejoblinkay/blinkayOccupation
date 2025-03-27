# Imagen base para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagen para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar los archivos de proyecto y restaurar dependencias
COPY BlinkayOccupation.API/BlinkayOccupation.API.csproj BlinkayOccupation.API/
COPY BlinkayOccupation.Application/BlinkayOccupation.Application.csproj BlinkayOccupation.Application/
COPY BlinkayOccupation.Domain/BlinkayOccupation.Domain.csproj BlinkayOccupation.Domain/
COPY BlinkayOccupation.Infrastructure/BlinkayOccupation.Infrastructure.csproj BlinkayOccupation.Infrastructure/

RUN dotnet restore BlinkayOccupation.API/BlinkayOccupation.API.csproj

# Copiar el resto del código y compilar
COPY . .
WORKDIR /src/BlinkayOccupation.API
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlinkayOccupation.API.dll"]
