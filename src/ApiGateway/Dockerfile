FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet publish "src/ApiGateway/ApiGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "ApiGateway.dll"]