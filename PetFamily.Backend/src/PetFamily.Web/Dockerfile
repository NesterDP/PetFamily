FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PetFamily.Web/PetFamily.Web.csproj", "src/PetFamily.Web/"]
COPY ["src/Accounts/PetFamily.Accounts.Application/PetFamily.Accounts.Application.csproj", "src/Accounts/PetFamily.Accounts.Application/"]
COPY ["src/Shared/PetFamily.Core/PetFamily.Core.csproj", "src/Shared/PetFamily.Core/"]
COPY ["src/Shared/PetFamily.SharedKernel/PetFamily.SharedKernel.csproj", "src/Shared/PetFamily.SharedKernel/"]
COPY ["src/Accounts/PetFamily.Accounts.Domain/PetFamily.Accounts.Domain.csproj", "src/Accounts/PetFamily.Accounts.Domain/"]
COPY ["src/Accounts/PetFamily.Accounts.Contracts/PetFamily.Accounts.Contracts.csproj", "src/Accounts/PetFamily.Accounts.Contracts/"]
COPY ["src/Accounts/PetFamily.Accounts.Presentation/PetFamily.Accounts.Presentation.csproj", "src/Accounts/PetFamily.Accounts.Presentation/"]
COPY ["src/Accounts/PetFamily.Accounts.Infrastructure/PetFamily.Accounts.Infrastructure.csproj", "src/Accounts/PetFamily.Accounts.Infrastructure/"]
COPY ["src/Shared/PetFamily.Framework/PetFamily.Framework.csproj", "src/Shared/PetFamily.Framework/"]
COPY ["src/Species/PetFamily.Species.Application/PetFamily.Species.Application.csproj", "src/Species/PetFamily.Species.Application/"]
COPY ["src/Volunteers/PetFamily.Volunteers.Contracts/PetFamily.Volunteers.Contracts.csproj", "src/Volunteers/PetFamily.Volunteers.Contracts/"]
COPY ["src/Species/PetFamily.Species.Domain/PetFamily.Species.Domain.csproj", "src/Species/PetFamily.Species.Domain/"]
COPY ["src/Species/PetFamily.Species.Contracts/PetFamily.Species.Contracts.csproj", "src/Species/PetFamily.Species.Contracts/"]
COPY ["src/Species/PetFamily.Species.Presentation/PetFamily.Species.Presentation.csproj", "src/Species/PetFamily.Species.Presentation/"]
COPY ["src/Volunteers/PetFamily.Volunteers.Application/PetFamily.Volunteers.Application.csproj", "src/Volunteers/PetFamily.Volunteers.Application/"]
COPY ["src/Volunteers/PetFamily.Volunteers.Domain/PetFamily.Volunteers.Domain.csproj", "src/Volunteers/PetFamily.Volunteers.Domain/"]
COPY ["src/Volunteers/PetFamily.Volunteers.Infrastructure/PetFamily.Volunteers.Infrastructure.csproj", "src/Volunteers/PetFamily.Volunteers.Infrastructure/"]
COPY ["src/Volunteers/PetFamily.Volunteers.Presentation/PetFamily.Volunteers.Presentation.csproj", "src/Volunteers/PetFamily.Volunteers.Presentation/"]
COPY ["src/Species/PetFamily.Species.Infrastructure/PetFamily.Species.Infrastructure.csproj", "src/Species/PetFamily.Species.Infrastructure/"]
COPY ["src/VolunteerRequests/PetFamily.VolunteerRequests.Presentation/PetFamily.VolunteerRequests.Presentation.csproj", "src/VolunteerRequests/PetFamily.VolunteerRequests.Presentation/"]
COPY ["src/VolunteerRequests/PetFamily.VolunteerRequests.Application/PetFamily.VolunteerRequests.Application.csproj", "src/VolunteerRequests/PetFamily.VolunteerRequests.Application/"]
COPY ["src/VolunteerRequests/PetFamily.VolunteerRequests.Domain/PetFamily.VolunteerRequests.Domain.csproj", "src/VolunteerRequests/PetFamily.VolunteerRequests.Domain/"]
COPY ["src/Discussions/PetFamily.Discussions.Contracts/PetFamily.Discussions.Contracts.csproj", "src/Discussions/PetFamily.Discussions.Contracts/"]
COPY ["src/VolunteerRequests/PetFamily.VolunteerRequests.Contracts/PetFamily.VolunteerRequests.Contracts.csproj", "src/VolunteerRequests/PetFamily.VolunteerRequests.Contracts/"]
COPY ["src/VolunteerRequests/PetFamily.VolunteerRequests.Infrastructure/PetFamily.VolunteerRequests.Infrastructure.csproj", "src/VolunteerRequests/PetFamily.VolunteerRequests.Infrastructure/"]
COPY ["src/Discussions/PetFamily.Discussions.Presentation/PetFamily.Discussions.Presentation.csproj", "src/Discussions/PetFamily.Discussions.Presentation/"]
COPY ["src/Discussions/PetFamily.Discussions.Application/PetFamily.Discussions.Application.csproj", "src/Discussions/PetFamily.Discussions.Application/"]
COPY ["src/Discussions/PetFamily.Discussions.Domain/PetFamily.Discussions.Domain.csproj", "src/Discussions/PetFamily.Discussions.Domain/"]
COPY ["src/Discussions/PetFamily.Discussions.Infrastructure/PetFamily.Discussions.Infrastructure.csproj", "src/Discussions/PetFamily.Discussions.Infrastructure/"]
RUN dotnet restore "src/PetFamily.Web/PetFamily.Web.csproj"
COPY . .
WORKDIR "/src/src/PetFamily.Web"
RUN dotnet build "PetFamily.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PetFamily.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetFamily.Web.dll"]
