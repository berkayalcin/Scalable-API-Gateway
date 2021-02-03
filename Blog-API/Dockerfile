FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY ["Blog-API/Blog-API.csproj", "./"]
COPY . .
WORKDIR /src/.
RUN dotnet restore "./Blog-API.csproj"
RUN dotnet build "Blog-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog-API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "blog_api.dll"]
