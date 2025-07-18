FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем все проекты
COPY ["CargoGateway.API/CargoGateway.API.csproj", "CargoGateway.API/"]
COPY ["CargoGateway.Core/CargoGateway.Core.csproj", "CargoGateway.Core/"]
COPY ["CargoGateway.Infrastructure/CargoGateway.Infrastructure.csproj", "CargoGateway.Infrastructure/"]

# Восстанавливаем зависимости
RUN dotnet restore "CargoGateway.API/CargoGateway.API.csproj"

# Копируем весь исходный код
COPY . .

# Собираем и публикуем
WORKDIR "/src/CargoGateway.API"
RUN dotnet build "CargoGateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CargoGateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CargoGateway.API.dll"]