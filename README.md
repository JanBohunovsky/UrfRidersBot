![build](https://github.com/JanBohunovsky/UrfRidersBot/workflows/build/badge.svg)

# Configuration
Example configuration:  
```json5
{
    "Discord": {
        "Token": "...",
        "ClientId": "...",
        "ClientSecret": "..."
    },
    "RiotGames": {
        "ApiKey": "..."
    },
    "Serilog": {
        "MinimumLevel": {
            "Default": "Warning"
        }
    },
    "ConnectionStrings": {
        "UrfRidersData": "<postgres connection string>"
    }
}
```
To see more configuration options, check the [configuration models](/src/UrfRidersBot.Core/Common/Configuration).