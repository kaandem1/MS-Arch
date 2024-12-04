# For .NET 6
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MCMS.csproj", "./"]
RUN dotnet restore "./MCMS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MCMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MCMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "MCMS.dll"]
