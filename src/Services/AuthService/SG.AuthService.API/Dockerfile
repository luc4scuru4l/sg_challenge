FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/AuthService/SG.AuthService.API/SG.AuthService.API.csproj", "src/Services/AuthService/SG.AuthService.API/"]
COPY ["src/Services/AuthService/SG.AuthService.Application/SG.AuthService.Application.csproj", "src/Services/AuthService/SG.AuthService.Application/"]
COPY ["src/Services/AuthService/SG.AuthService.Domain/SG.AuthService.Domain.csproj", "src/Services/AuthService/SG.AuthService.Domain/"]
COPY ["src/Services/AuthService/SG.AuthService.Infrastructure/SG.AuthService.Infrastructure.csproj", "src/Services/AuthService/SG.AuthService.Infrastructure/"]
RUN dotnet restore "src/Services/AuthService/SG.AuthService.API/SG.AuthService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/AuthService/SG.AuthService.API"
RUN dotnet build "./SG.AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SG.AuthService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SG.AuthService.API.dll"]
