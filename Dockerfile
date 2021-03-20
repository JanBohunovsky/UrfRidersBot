FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 80
COPY out .
ENTRYPOINT ["dotnet", "UrfRidersBot.dll"]
