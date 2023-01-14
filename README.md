# Stationeers.Addons
<a href="https://discord.gg/b6kFrUATdm"><img src="https://discordapp.com/api/guilds/795601381956124693/widget.png"/></a></br>

**Stationeers.Addons** is a fully-fledged modding framework for [Stationeers](https://store.steampowered.com/app/544550/Stationeers/). Working just like the standard mods (XML) but with scripting and custom-content support!

**Current status:** Available for early testing.<br>

## Download & Installation
### Windows
If you have already installed the Addons, make sure to run file verification through Steam before installing a new version (go to Steam, click RMB on the game, open **Properties**, go to **Local Files** and click on **Verify integrity of game files...**)!
 
Go to [Releases](https://github.com/Erdroy/Stationeers.Addons/releases), select latest release and download zip file named 'Stationeers.Addons-vX.X-X.zip'. Now go to Steam, click RMB on the game, open **Properties**, go to **Local Files** and click on **BROWSE LOCAL FILES**. It should open new window for you. Next, you have to open the downloaded zip and drag all of its contents into the game folder (`AddonManager` folder and `version.dll`). And that's it! Enjoy your mods!

***Note:** After you've subscribed to an addon on the workshop, you have to restart the game. This will be improved in the future.*

### Linux (client, not tested for servers)
*(this is in beta, might not work)*
Installing:
1) Do the same unzipping as in windows (extract all zip content on the game installation root folder)
2) In a shell, navigate to the Addons folder you've just extracted
3) Run `mono Stationeers.Addons.Patcher.exe`
4) Enjoy (hopefully)

Updating:
1) Like in windows, verify the files from steam
2) Extract new version of this package
3) Run the exe (step 3 of `Installing`)


## Links
* [Discord](https://discord.gg/b6kFrUATdm)
* [Trello](https://trello.com/b/zSHKh2XO/stationeersaddons)
* [Official Github](https://github.com/Erdroy/Stationeers.Addons)

## Building 
`Visual Studio 2019` is required and `Visual Studio Tools for Unity` installation is recommended. 
* Open `Source/Stationeers.VS.props` file, and set the path to your game installation directory (has to end with a backslash). 
* Open `Source/Stationeers.Addons.sln` and start playing with it! See [Creating addons](https://github.com/Erdroy/Stationeers.Addons#creating-addons) to find out more.

## Creating addons
If you want to create your own addon, read here: [CREATING-ADDONS](Docs/CREATING-ADDONS.md).

## Debugging addons
If you want to debug your own addon, read here: [DEBUGGING-ADDONS](Docs/DEBUGGING-ADDONS.md).

## Addons
* [Example Mod](https://steamcommunity.com/sharedfiles/filedetails/?id=2308921579) by Erdroy
* [SEGI - Sonic Ether Global Illumination](https://steamcommunity.com/sharedfiles/filedetails/?id=2308956244) by Erdroy
* [NormalQuarry](https://steamcommunity.com/sharedfiles/filedetails/?id=2621212864) by mehanic321
* [StationeersAddonsHelper](https://steamcommunity.com/sharedfiles/filedetails/?id=2798686984) by mehanic321
* [Better Satellite Dish](https://steamcommunity.com/sharedfiles/filedetails/?id=2747308919) by silentdeth

*DM **`Erdroy#0001`** on Discord, if you would like to get your mod featured here.*

## Dependecies
* Mono.Cecil
* Harmony

## Contributions
We're accepting pull requests, look at our Trello board to find tasks that have not been completed, yet.
You can hop in, take some and help us evolve this modding framework!

Although, we want to keep mod consistency, we're suggesting to not release modified copy of this software on your own.
Anyway, this is certainly legal, if you would like to do that.

## License
MIT

___
***Stationeers.Addons** is not affiliated with RocketWerkz. All trademarks and registered trademarks are the property of their respective owners.*<br>
***Stationeers.Addons** (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors*
