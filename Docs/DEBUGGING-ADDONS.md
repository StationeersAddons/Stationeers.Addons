# Stationeers.Addons

## Debugging addons.
Debugging addons come in handy, when you want to check if your plugins are functioning correctly, looking for bugs etc.
To get it working, you have to make a little bit of setup. Follow these steps:
1. Make sure that you have `Visual Studio Tools for Unity` installed.
2. Go to your game installation directory and find `rocketstation_Data/boot.config` file.
    * Replace `player-connection-debug=0` with `player-connection-debug=1`.
3. Create an empty file called `addons-debugging.enable`. It must be located where the game's exe file is.
4. Go to the Visual Studio and change the output directory of your addon to point to a path: `C:\Users\%username%\Documents\My Games\Stationeers\mods\MODNAME-Debug` (Replace *MODNAME* with your mod name, i.e. *MyFirstAddon* - then it should be named *MyFirstAddon-Debug*).
The -Debug post-fix is necessary.
5. Rebuild your addon and make sure, that the `MODNAME-Debug` contains `MODNAME.dll` file.
6. Setup some breakpoints.
7. Startup your game and at this moment, you can attach Unity Debugger (as soon as game shows up in the list). To do so, go to `Debug > Attach Unity Debugger` and keep hitting Refresh every ~5 seconds untill the game shows up. If it is there, just connect to it and you can start debugging.

___
***Stationeers.Addons** is not affiliated with RocketWerkz. All trademarks and registered trademarks are the property of their respective owners.*<br>
***Stationeers.Addons** (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors*
