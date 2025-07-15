# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/MusicStore.Api/MusicStore.Api.csproj", "src/Api/MusicStore.Api/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Core/BuildingBlocks.Core.csproj", "src/BuildingBlocks/BuildingBlocks.Core/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Web/BuildingBlocks.Web.csproj", "src/BuildingBlocks/BuildingBlocks.Web/"]
COPY ["src/Modules/Catalog/MusicStore.Modules.Catalog/MusicStore.Modules.Catalog.csproj", "src/Modules/MusicStore.Modules.Catalog/"]
COPY ["src/Modules/Customer/MusicStore.Modules.Customer/MusicStore.Modules.Customer.csproj", "src/Modules/MusicStore.Modules.Customer/"]
COPY ["src/Modules/Identity/MusicStore.Modules.Identity/MusicStore.Modules.Identity.csproj", "src/Modules/MusicStore.Modules.Identity/"]
COPY ["src/Modules/Order/MusicStore.Modules.Order/MusicStore.Modules.Order.csproj", "src/Modules/MusicStore.Modules.Order/"]
COPY ["src/Modules/ShoppingCart/MusicStore.Modules.ShoppingCart/MusicStore.Modules.ShoppingCart.csproj", "src/Modules/MusicStore.Modules.ShoppingCart/"]
RUN dotnet restore "./src/Api/MusicStore.Api/MusicStore.Api.csproj"
COPY . .
WORKDIR "/src/src/Api/MusicStore.Api"
RUN dotnet build "./MusicStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MusicStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicStore.Api.dll"]