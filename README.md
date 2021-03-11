![build](https://github.com/JanBohunovsky/UrfRidersBot/workflows/build/badge.svg)

# Configuration
Example configuration:  
```json5
{
    "Discord": {
        "Prefix": "!",
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
To see more configuration options, check the [configuration models](/src/UrfRidersBot.Discord/Configuration).


# Changelog
## 2.0.0
### Added bot version
- The bot version uses following format: `Major.Minor.Patch`
- **Major** - *Big changes that the user has to learn to use the bot.*
  - New/reworked command groups (e.g. Clash)
  - New/reworked systems (e.g. Guild Rank)
- **Minor** - *Small changes that will affect how the user uses the bot.*
  - New/updated commands
  - New/updated services
- **Patch** - *Things that user is usually not affected by.*
  - Bug fixes
  - Text improvements (e.g. command outputs, fixed typos)
  - Changes to command documentation (e.g. parameter description)
  - Internal changes (e.g. updated nuget package)
- With this change, the version 2.0 is "complete". From now on I'm gonna be using this versioning scheme while implementing new or old features.

## 2.0.0-dev3
### Added color roles
- Users can give themselves a role with custom color using the new `color` command.


## 2.0.0-dev2
### Added Messages API endpoint
- Currently supports only message sending.

### Re-added Reaction Roles
- Now works like a toggle
  - The bot will remove your reaction every time.
  - If you already have the role, the bot will remove it.
  - If you don't have the role, the bot will give it to you.
  - The reason for this change was that I have no way of adding other user's reactions if they already have the role so the reactions would not match the number of users with that role.


## 2.0.0-dev1
- This version was made from scratch. Some features from previous versions may be missing.
- Added help command to list all available commands (thanks to DSharpPlus).

### Backend
- Changed discord wrapper library to [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus).
- No longer a console application, now it's hosted inside ASP.NET Web API project.
  - This will allow me to create endpoints that can control the bot.
  
### Improved Auto Voice
- Changed the name of last/new voice channel to be more clear (now called `➕ New Voice Channel`).
- The channel name now gets updated when users change their activity instead of every 5 seconds.

### Improved Settings
- Settings commands are now implemented manually.
  - This means that for every new setting added, we need to add a new command.
  - This decision has been made to support argument parsing (i.e. if you're setting a role, you can just mention it instead of posting its ID).

### New public command
- `ask <question>`
  - This will post an embed with user's question and with two reactions (yes and no).
  - Mentioned roles are separated from the embed's content (so they will get pinged).


## 1.4
### Added README
- Moved example configuration file here.
- Moved changelog here.

### Added Clash Module
- Mark channel as "Clash channel" by running command `clash init`,
this will post upcomming tournaments, pin this message and keep it updated, and change channel topic to show countdown to next tournament.
- The module will notify servers at the start of the week if there are tournaments available that week.
- The module will notify servers in the morning if there is a tournament that day.
- Team builder is planned in the future.


### Refactored code structure
- Moved modules and its components into specific folders (from Data, Modules and Services).
- This should be more readable now and in the future with more modules.
- `Services` folder now contains basic services that can be used by all modules (e.g. LogService).




## 1.3.5
### Auto Voice Module
- Temporary solution for invalid channel ID in voice channel list




## 1.3.4
### Auto Voice Module
- Fixed gateway blocking in UserVoiceStateUpdated event




## 1.3.3
### COVID-19 Module
- Updated to new layout




## 1.3.2
### COVID-19 Module
- Updated to new layout
- Improved embed
- Added more aliases
- Moved cache data to each server (to ServerData)




## 1.3.1
- Changed help command into tech support command.
- Cleaned up code.




## 1.3
- Added Interactive Service for interacting with user via reactions or replies (replies not implemented yet).
- Unknown subcommands now show error.

### Added Reaction Roles Module
- Allows you to set roles via reactions.


### Auto Voice Module
- Added module status to help command.
- Fixed bug that caused the module to not update channel names periodically.

### COVID-19 Module
- Added support for negative delta.




## 1.2.2
- Improved RequireLevel error messages.
- Improved error messages (now uses embed).

- Removed WithWarning extension from Embed Builder and changed WithError color to yellow.


### Core Module
- Added embed command that creates embed from json.

### Settings
- Added description for permission roles.




## 1.2.1
- Added PermissionLevel (Everyone < Member < Moderator < Admin < Owner).  
- Added RequireLevel attribute that works like RequireUserPermission except it checks defined roles.  
- Added WithSuccess, WithWarning and WithError extensions to Embed Builder.  

- Renamed Info module to Core.

- Fixed a problem in services with periodic update that would cause to run multiple updates at the same time.


### Core Module
- Added alias "version" to Info command.

### COVID-19 Module
- Triggers typing when the command is executed.  
- If the last message in COVID-19 Channel is posted by this service, then it will be updated instead.




## 1.2
- Added changelog.

### ServerSettings
- Moved to Services.  
- Added static function that returns settings for all servers.  

### COVID-19 Module
- Changed a little bit how periodic update works.  
- Added some log messages that should help with debugging in the future.  

### Auto Voice Module
- Big part of module rewritten.  
- Now stores all channels which makes it able to recover after restart.  
- If there is a collision between most played games and one of them is one of programming apps
then the name will be set to General instead of Gaming.  
