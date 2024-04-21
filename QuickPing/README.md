# QuickPing
[ExploreTogether](https://valheim.thunderstore.io/package/Rolo/ExploreTogether/)'s ping feature remastered. 

## Install Instruction
Ping position and markers will always work but must be installed on all clients if you want everyone able to see pinged object's name display.

## Features
- Use T (by default, see config) to **ping what you point**, name of pointed object/creature is displayed in the world and on the minimap to all players.
  Some objects automatically add a pin on map if a player ping them (Copper, Silver, Dungeons, Portals, Berries etc.)
- Use G (by default, see config) to **force add a marker** (exclamation point by default if not filtered) **on map** with pointed object's name.
- If a pinned object is destroyed, it will also destroy its attached marker.
- ***(New)*** Use Alt + Ping (by default, see config) and enter a custom name to **ping an object**.
- Change text colors (chat, ping, shout, whisper, names) in config 
- Change already existing Portal pin by pinging the portal again, it will change the marker's name to portal tag.

## What's Changed
* Hotfix/patch 0.217.46 by @Vodianoi in https://github.com/Vodianoi/QuickPingMod/pull/116

**Full Changelog**: https://github.com/Vodianoi/QuickPingMod/compare/1.5.10...1.5.11

## Configuration
The settings for QuickPing can be accessed in the BepInEx config file located in the `Valheim/BepInEx/config` folder. The settings include:

- **Bindings**
  - **PingKey**: The keybind to trigger a ping where you are looking (default: T)
  - **PingEverythingKey**: The keybind to add a pin on minimap to whatever you're looking at (default: G)
  - **RenameKey**: The keybind to rename a ping (default: T + LeftAlt)

- **General**
  - **PingWhereLooking**: Create a ping where you are looking when you press the Ping key (default: true)
  - **AddPinOnMap**: Add a marker on map when useful resources are pinged (ignored by G) (default: true)
  - **ClosestPinRange**: Minimum distance between objects to pin/replace portal tag (default: 2)
  - **DefaultPinType**: Default marker when forcing adding a pin on map (default: RandomEvent (!))
  
- **Colors**
  - **PlayerColor**: Color for Player name in pings/messages (default: green)
  - **ShoutColor**: Color for Shout ping (default: yellow)
  - **WhisperColor**: Color for Whisper ping (default: white with 75% opacity)
  - **PingColor**: Color for "Ping" ping (default: light blue)
  - **DefaultColor**: Default color (default: white)
  


## Known issues and ideas 
You can find the github and contribute by adding an issue at: [Github](https://github.com/Vodianoi/QuickPingMod)
Please report any bug !

## Downloads
*___[Thunderstore](https://valheim.thunderstore.io/package/Atopy/QuickPing/)___*

*___[Nexus](https://www.nexusmods.com/valheim/mods/2033)___*
