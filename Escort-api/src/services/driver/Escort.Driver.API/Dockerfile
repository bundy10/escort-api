﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Escort.Driver.API/Escort.Driver.API.csproj", "Escort.Driver.API/"]
RUN dotnet restore "Escort.Driver.API/Escort.Driver.API.csproj"
COPY . .
WORKDIR "/src/Escort.Driver.API"
RUN dotnet build "Escort.Driver.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Escort.Driver.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Escort.Driver.API.dll"]
