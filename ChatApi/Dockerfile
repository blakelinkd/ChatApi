FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the CSPROJ file and restore any dependencies
COPY ChatApi.csproj .
RUN dotnet restore

# Copy the rest of your app's source code
COPY . .
RUN dotnet build "ChatApi.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ChatApi.csproj" -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApi.dll"]