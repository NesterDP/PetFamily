FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

ARG NUGET_USERNAME
ARG NUGET_PASSWORD
ARG NUGET_SOURCE="https://nuget.pkg.github.com/NesterDP/index.json"

RUN dotnet nuget add source $NUGET_SOURCE \
    --name "GitHub" \
    --username $NUGET_USERNAME \
    --password $NUGET_PASSWORD \
    --store-password-in-clear-text

WORKDIR /src
COPY ["src/NotificationService/NotificationService.csproj", "src/NotificationService/"]
COPY ["src/NotificationService.Contracts/NotificationService.Contracts.csproj", "src/NotificationService.Contracts/"]
RUN dotnet restore "src/NotificationService/NotificationService.csproj"
COPY . .
WORKDIR "/src/src/NotificationService"
RUN dotnet build "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
