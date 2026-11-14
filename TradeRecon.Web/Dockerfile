FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TradeRecon.Web/TradeRecon.Web.csproj", "TradeRecon.Web/"]
COPY ["TradeRecon.Core/TradeRecon.Core.csproj", "TradeRecon.Core/"]

RUN dotnet restore "TradeRecon.Web/TradeRecon.Web.csproj"

COPY . .

WORKDIR "/src/TradeRecon.Web"

RUN dotnet publish \
    "TradeRecon.Web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TradeRecon.Web.dll"]
