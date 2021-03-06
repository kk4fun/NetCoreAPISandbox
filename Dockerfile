#build container
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-buster as build

WORKDIR /build
COPY . .
RUN dotnet run -p build/build.csproj

#runtime container
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-buster-slim

COPY --from=build /build/publish /app
WORKDIR /app

EXPOSE 5000

ENTRYPOINT ["dotnet", "NetCoreApiSandbox.dll"]
