#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Accessors/Spinoza.Backend.Accessor.QuestionCatalog/Spinoza.Backend.Accessor.QuestionCatalog.csproj", "Accessors/Spinoza.Backend.Accessor.QuestionCatalog/"]
COPY ["Crosscutting/Spinoza.Backend.Crosscutting.CosmosDBWrapper/Spinoza.Backend.Crosscutting.CosmosDBWrapper.csproj", "Crosscutting/Spinoza.Backend.Crosscutting.CosmosDBWrapper/"]
RUN dotnet restore "Accessors/Spinoza.Backend.Accessor.QuestionCatalog/Spinoza.Backend.Accessor.QuestionCatalog.csproj"
COPY . .
WORKDIR "/src/Accessors/Spinoza.Backend.Accessor.QuestionCatalog"
RUN dotnet build "Spinoza.Backend.Accessor.QuestionCatalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spinoza.Backend.Accessor.QuestionCatalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spinoza.Backend.Accessor.QuestionCatalog.dll"]
