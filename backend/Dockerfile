# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["src/TradingCenter.Domain/TradingCenter.Domain.csproj", "src/TradingCenter.Domain/"]
COPY ["src/TradingCenter.Repository/TradingCenter.Repository.csproj", "src/TradingCenter.Repository/"]
COPY ["src/TradingCenter.Services/TradingCenter.Services.csproj", "src/TradingCenter.Services/"]
COPY ["src/TradingCenter.Api/TradingCenter.Api.csproj", "src/TradingCenter.Api/"]

RUN dotnet restore "src/TradingCenter.Api/TradingCenter.Api.csproj"

COPY . .
WORKDIR "/src/src/TradingCenter.Api"
RUN dotnet build "TradingCenter.Api.csproj" -c Release -o /app/build
RUN dotnet publish "TradingCenter.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TradingCenter.Api.dll"]
