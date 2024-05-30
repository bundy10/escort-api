FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Escort.Client.API/Escort.User.API.csproj", "Escort.User.API/"]
COPY ["Escort.Client.Application/Escort.User.API.csproj", "Escort.User.Application/"]
COPY ["Escort.Client.Domain/Escort.User.API.csproj", "Escort.User.Domain/"]
COPY ["Escort.Client.Infrastructure/Escort.User.API.csproj", "Escort.User.Infrastructure/"]
RUN dotnet restore "Escort.Client.API/Escort.Client.API.csproj"
COPY . .
WORKDIR "/src/Escort.Client.API"
RUN dotnet build "Escort.Client.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Escort.Client.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Escort.Client.API.dll"]
