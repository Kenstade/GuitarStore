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
COPY ["src/Api/GuitarStore.Api/GuitarStore.Api.csproj", "src/Api/GuitarStore.Api/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Core/BuildingBlocks.Core.csproj", "src/BuildingBlocks/BuildingBlocks.Core/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Web/BuildingBlocks.Web.csproj", "src/BuildingBlocks/BuildingBlocks.Web/"]
COPY ["src/Modules/GuitarStore.Modules.Catalog/GuitarStore.Modules.Catalog.csproj", "src/Modules/GuitarStore.Modules.Catalog/"]
COPY ["src/Modules/GuitarStore.Modules.Customers/GuitarStore.Modules.Customers.csproj", "src/Modules/GuitarStore.Modules.Customers/"]
COPY ["src/Modules/GuitarStore.Modules.Identity/GuitarStore.Modules.Identity.csproj", "src/Modules/GuitarStore.Modules.Identity/"]
COPY ["src/Modules/GuitarStore.Modules.Ordering/GuitarStore.Modules.Ordering.csproj", "src/Modules/GuitarStore.Modules.Ordering/"]
COPY ["src/Modules/GuitarStore.Modules.ShoppingCarts/GuitarStore.Modules.ShoppingCarts.csproj", "src/Modules/GuitarStore.Modules.ShoppingCarts/"]
RUN dotnet restore "./src/Api/GuitarStore.Api/GuitarStore.Api.csproj"
COPY . .
WORKDIR "/src/src/Api/GuitarStore.Api"
RUN dotnet build "./GuitarStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GuitarStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuitarStore.Api.dll"]