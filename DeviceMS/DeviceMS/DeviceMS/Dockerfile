# For .NET 6
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeviceMS.csproj", "./"]
RUN dotnet restore "./DeviceMS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DeviceMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeviceMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeviceMS.dll"]
