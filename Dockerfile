# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ChatApi/ChatApi.csproj ./ChatApi/
RUN dotnet restore

# copy everything else and build app
COPY ChatApi/. ./ChatApi/
WORKDIR /source/ChatApi
RUN dotnet publish -c release -o /ChatApi --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ChatApi
COPY --from=build /ChatApi ./
ENTRYPOINT ["dotnet", "ChatApi/bin/Release/net8.0/ChatApi.dll"]