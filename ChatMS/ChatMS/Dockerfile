# For .NET 6
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatMS.csproj", "./"]
RUN dotnet restore "./ChatMS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ChatMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatMS.dll"]
