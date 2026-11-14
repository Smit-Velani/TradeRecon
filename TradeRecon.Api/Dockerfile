FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TradeRecon.Api/TradeRecon.Api.csproj", "TradeRecon.Api/"]
COPY ["TradeRecon.Application/TradeRecon.Application.csproj", "TradeRecon.Application/"]
COPY ["TradeRecon.Core/TradeRecon.Core.csproj", "TradeRecon.Core/"]
COPY ["TradeRecon.Infrastructure/TradeRecon.Infrastructure.csproj", "TradeRecon.Infrastructure/"]

RUN dotnet restore "TradeRecon.Api/TradeRecon.Api.csproj"

COPY . .

WORKDIR "/src/TradeRecon.Api"

RUN dotnet publish \
    "TradeRecon.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TradeRecon.Api.dll"]
