# Stationeers.Addons
<a href="https://discord.gg/b6kFrUATdm"><img src="https://discordapp.com/api/guilds/795601381956124693/widget.png"/></a></br>

**Stationeers.Addons** is a fully-fledged modding framework for [Stationeers](https://store.steampowered.com/app/544550/Stationeers/).
It is responsible for scripting support (compilation and loading) and custom content loading (via asset boundles).

**Current status:** Available for testing.<br>
*Full version ETA: **Q1 2021***

## Download & Installation
Go to [Releases](https://github.com/Erdroy/Stationeers.Addons/releases) and select latest release and download zip file named 'Stationeers.Addons-vX.X-X.zip'. Now go to Steam, click RMB on the game, open **Properties**, go to **Local Files** and click on **BROWSE LOCAL FILES**. It should open new window for you. Next, you have to open the downloaded zip and drag all of its contents into the game folder (`AddonManager` folder). Finally, run `AddonManager/Stationeers.Addons.Patcher.exe` and it's done. You can now open up the game via Steam and enjoy the mods!

***Note:** Our mod loader will stop working when new update is released, in that case you will have to run `AddonManager/Stationeers.Addons.Patcher.exe` again. This will be improved soon.*
***Note:** After you've subscribed to an addon on the workshop, you have to restart the game. This will be improved in the future.*

## Links
* [Discord](https://discord.gg/b6kFrUATdm)
* [Trello](https://trello.com/b/zSHKh2XO/stationeersaddons)
* [Official Github](https://github.com/Erdroy/Stationeers.Addons)

## Making a mod
Comming soon.

## Building
Visual Studio 2019 is required.
Please read Libraries [README](Libraries/Stationeers/README.md) file.
Open `Source/Stationeers.Addons.sln` and start playing with it!

## Debugging
Comming soon.
Note: boot.config, unity and output paths

## Dependecies
* Mono.Cecil
* Harmony

## Mods
* [Example Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2308921579) by Erdroy
* [SEGI - Sonic Ether Global Illumination](https://steamcommunity.com/sharedfiles/filedetails/?id=2308956244) by Erdroy

*DM **Erdroy#0001** on Discord, if you would like to get your mod featured here.*

## Contributions
We're accepting pull requests, look at our Trello board to find tasks that have not been completed, yet.
You can hop in, take some and help us evolve this modding framework!

Although, we want to keep mod consistency, we're suggesting to not release modified copy of this software on your own.
Anyway, this is certainly legal, if you would like to do that.

## License
MIT

___
***Stationeers.Addons** is not affiliated with RocketWerkz. All trademarks and registered trademarks are the property of their respective owners.*<br>
***Stationeers.Addons** (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors*
