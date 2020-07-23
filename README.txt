****************
RPG GAME PROJECT
****************

AUTHOR: Aidan Sakovits
Credit to user Jaykul on Stackoverflow for the ConsoleHelper class.

README UPDATED: 17/07/2020

README: This project was made by Aidan Sakovits for assessment by the Academy of Interactive Entertainment.
It demonstrates an inventory and trading system as would be found in an RPG style console game.

BUILD: Open the solution in Visual Studio 2019, and create a release build. All necessary files and file
structures are handled automatically.

EXECUTION: After build, execution is performed simply by running the RPGGame.exe file.

USE: Type HELP ingame for a list of commands.

SUPER ACCESS COMMAND: "GIVE ME GOOD GRADES" (excluding quotes)

FILE MANIFEST:
	InventoriesData
		INDEX.dat
		INVENTORY.dat
		MERCHANT.dat
	Items
		Ammunition.cs
		Armour.cs
		Gold.cs
		Item.cs
		Miscellaneous.cs
		Potion.cs
		Weapon.cs
	Managers
		CommandManager.cs
		EntityManager.cs
		cs
	Tools
		ConsoleHelper.cs
		ImportExportTool.cs
		ParseTool.cs
		TestTool.cs
		cs
	Entity.cs
	Game.cs
	GameBoard.cs
	GameLoop.cs
	README.txt
	RPGGame.csproj
	RPGGame.sln


LICENSING: GNU General Public License
https:

REQUIREMENTS: Windows 10 (insider) build 1826 or higher
Support not guaranteed on other platforms or earlier builds

KNOWN BUGS:
Some characters such as ' [(char)39] cause problems.
Use of keywords in item names causes problems.
When adding/renaming, dummy words and particles should be avoided.
Lines that overflow may cause the map to go offscreen.