FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/AccountService/SG.AccountService.API/SG.AccountService.API.csproj", "src/Services/AccountService/SG.AccountService.API/"]
COPY ["src/Services/AccountService/SG.AccountService.Application/SG.AccountService.Application.csproj", "src/Services/AccountService/SG.AccountService.Application/"]
COPY ["src/Services/AccountService/SG.AccountService.Domain/SG.AccountService.Domain.csproj", "src/Services/AccountService/SG.AccountService.Domain/"]
COPY ["src/Services/AccountService/SG.AccountService.Infrastructure/SG.AccountService.Infrastructure.csproj", "src/Services/AccountService/SG.AccountService.Infrastructure/"]
RUN dotnet restore "src/Services/AccountService/SG.AccountService.API/SG.AccountService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/AccountService/SG.AccountService.API"
RUN dotnet build "./SG.AccountService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SG.AccountService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SG.AccountService.API.dll"]
