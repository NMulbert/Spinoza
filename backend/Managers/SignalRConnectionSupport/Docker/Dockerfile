#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#docker file was moved to a sub folder to mitigate a problem with compose: https://github.com/microsoft/DockerTools/issues/141#issuecomment-522822258

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Managers/SignalRConnectionSupport/SignalRConnectionSupport.csproj", "Managers/SignalRConnectionSupport/"]
RUN dotnet restore "Managers/SignalRConnectionSupport/SignalRConnectionSupport.csproj"
COPY . .
WORKDIR "/src/Managers/SignalRConnectionSupport"
RUN dotnet build "SignalRConnectionSupport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalRConnectionSupport.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
     HOST_CORS="*" \
    HOST_CORSCredentials=true

