FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY ["API-Gateway/API-Gateway.csproj", "./"]
RUN dotnet restore "./API-Gateway.csproj"
COPY . .
WORKDIR /src/.

RUN dotnet build "API-Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API-Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "api_gateway.dll"]
