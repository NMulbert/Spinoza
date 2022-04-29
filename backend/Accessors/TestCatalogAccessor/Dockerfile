#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Accessors/TestCatalogAccessor/Spinoza.Backend.Accessor.TestCatalog.csproj", "Accessors/TestCatalogAccessor/"]
COPY ["Crosscutting/Spinoza.Backend.Crosscutting.CosmosDBWrapper/Spinoza.Backend.Crosscutting.CosmosDBWrapper.csproj", "Crosscutting/Spinoza.Backend.Crosscutting.CosmosDBWrapper/"]
RUN dotnet restore "Accessors/TestCatalogAccessor/Spinoza.Backend.Accessor.TestCatalog.csproj"
COPY . .
WORKDIR "/src/Accessors/TestCatalogAccessor"
RUN dotnet build "Spinoza.Backend.Accessor.TestCatalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spinoza.Backend.Accessor.TestCatalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spinoza.Backend.Accessor.TestCatalog.dll"]