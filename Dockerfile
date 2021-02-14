# Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

# Run
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UrfRidersBot.WebAPI.dll"]
