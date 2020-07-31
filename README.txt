****************
RPG GAME PROJECT
****************

AUTHOR: Aidan Sakovits
Credit to user Jaykul on Stackoverflow for much of the ConsoleManager class.

README UPDATED: 31/07/2020

README: This project was made by Aidan Sakovits for assessment by the Academy of Interactive Entertainment.
It demonstrates an inventory and trading system as would be found in an RPG style console game.

BUILD: Open the solution in Visual Studio 2019, and create a release build with x64 CPU configuration. All
necessary files and should be handled automatically.

EXECUTION: After build, execution is performed simply by running the RPGGame.exe file.

USE: Type HELP ingame for a list of commands.

SUPER ACCESS COMMAND: "GIVE ME GOOD GRADES" (excluding quotes)

MARKING ASSISTANCE:
		A good place to get an overview of the program features is with the DEMO command, after
		attaining Super User status.

Items are implemented as a custom class, derived from a common base class.

		Yes, see Item and the other classes in the items folder.

Load the store and player inventory items from a text file when your program starts.You may use	
separate text files for each list.

		Store loads both inventories and entities from file, however all inventories
		are stored in one file, and all entities are stored in one file. This was to allow
		an arbitrary number of each to exist without spamming a folder full of hundreds of
		files. The requirement said "May", so I took some liberties.

Load the store and player inventory items from a text file when your program starts.
You may use separate text files for each list.

		See previous on loading.

Use text commands to list the inventory; buy, sell or inspect items.

		Yes.
		See the commands "ME" to view your own inventory.
		"INTERACT Merchant" (or other entity) while on the appropriate square. The Merchant
			is at (0, 3).
		Use BUY Dagger (or other available item) while in the Merchant's inventory to buy.
		Use SELL Dagger (or other available item) while in the Merchants inventory to sell.

Include a hidden command to allow a ‘super user’ to add new items to the store.
Implement this by providing an overridden constructor for each item.

		Yes???
		The Super User status is granted and rescinded via "GIVE ME GOOD GRADES".

		It allows the addition and removal of items from any inventory. That said, the item
		input format is a bit niche and program specific, so it may be lest to let the tailored
		feature demo activated by DEMO (while in super mode) display that for you.

		It saves your current gamestate to file, loads the TestInventories and TestEntities from file,
		reinitalizes the globals and managers, runs through a list of test commands, then
		ends and finally reloads your previously saved gamestate.

		The item constructors specifically are not individually overridden, though there are cases of
		overridden constructors and methods throughout the project. Instead, it uses an overridden parent
		class in conjunction with System.Reflection to create an arbitrary number of item types, without
		need to go through and hard code them all up individually, save for a few constructor 
		initializations and attribute requirement settings.

		But I have shown that I can override constructors, and go well beyond that, so I'm hoping
		I get the tick of approval there. If not, it will take me less than a day to throw a
		minimal remake of the project together for reassessment.

Perform and document unit tests on multiple sub-systems.
As you implement your text commands, or file I/O, test the system independently before
integrating it into your simulation. Ensure you document the unit tests.

		Yes. Over 100 successful unit tests with roughly 80% code coverage.

You must write clean, well formatted and well commented source code and provide a “readme”
or user document that explains how to compile (for example, which CPU configuration), execute
and operate your program.

		Readme provided, commenting is thorough except where things are super basic/self evident,
		in which case the bare explanation is given. Clean and well-formatted? I think so.

EXTENSION FEATURES:
Music, combat, arbitrary item data, a map/coordinate/movement system bound only by the integer limit, arbitrary
numbers of inventories and entities, equipping of items and their use in combat, and much more.

Use of variables, operators and expressions												Yes
Use of sequester, selection and iteration												Yes
Functions																				Yes
At least two instances of the use of arrays to store primitive or custom data types		Yes
Reading and writing to a text file														Yes
Two classes that each contain four instance variables									Yes
Multiple options for object construction												Yes
User-defined aggregation																Yes
Use of polymorphism once for code extensibility											Yes
Code documentation																		Yes

FILE MANIFEST:
	Entities
	        Enemy.cs
		Entity.cs
		Human.cs
		Imp.cs
		Wall.cs
	Items
		Ammunition.cs
		Armour.cs
		Gold.cs
		Item.cs
		Miscellaneous.cs
		Potion.cs
		Ring.cs
		Weapon.cs
	Managers
		CombatManager.cs
		CommandManager.cs
		ConsoleManager.cs
		EntityManager.cs
		ImportExportManager.cs
		InventoryManager.cs
		ParseManager.cs
		TextManager.cs
	Game.cs
	GameBoard.cs
	GameLoop.cs
	GlobalConstants.cs
	GlobalVariables.cs
	Inventory.cs
	MusicPlayer.cs
	Structs.cs
	Tune.csv
	Inventories.dat
	TestInventories.dat
	Entities.dat
	TestEntities.dat
	README.txt
	RPGGame.csproj
	RPGGame.sln

LICENSING: GNU General Public License
https://www.gnu.org/licenses/gpl-3.0.en.html

The music used is a modifier MIDI from Musescore. It is public domain: G.Ph.Telemann's Fantasy Nr 1

REQUIREMENTS: Windows 10 (insider) build 1826 or higher
Support not guaranteed on other platforms or earlier builds

KNOWN BUGS:
Some characters such as ' [(char)39] cause problems.
Some characters are also stored funny within the input, but alphanumerics should be fine.
Use of keywords in item names causes problems.
When adding/renaming, dummy words and particles should be avoided.
Lines that overflow may cause the map to go offscreen.
Spawning does not occur as expected.
Some flickering, usually on map respawn, due to console limitations.
The music can slow down/distort if a thread is working too hard.
The music notes have audible (annoying) clicks, due to console.beep limitations.

UNIT TESTING:
For unit testing, the Inventories.dat, TestInventories.dat, Entities.dat, TestEntities.dat and Tune.csv
should be copied into the bin/debug folder for the unit tests. Also, use MS tests as the program is set
up to recognise that in the Assemblies and skip certain parts of the code that would otherwise cause
hangs or crashes.
When last I checked, I was at just over 80% code coverage.

MORE:
This project grew very organically, which I suppose is a nice way of describing rampant scope creep.
Some of the methods I made to make my life easier later on, and then forgot about. Some things would have
gone much, much better if I had structured them out in advance with more Unit Driven testing, or even
just UML diagrams. But such is life. It's not as polished as I would have hoped, but it's functional and
there's some stuff in here I'm pretty proud of.
I learned about Regex, DLL importing, Reflection, Properties and more. I pushed myself on this, and whilst
I'll be glad to be past thethe 160+ hours I've dropped into it in 3 weeks, I feel like I've learned a
whole lot.
Regarding the reflection: It makes some of the constructors list no internal references (for obvious 
reasons), but more irritatingly it forces some of my constructors to have junk parameters, that are not
used, but that must still be there. Is there a clean way to state in my parameter list that I'm tossing
them? I don't know. Hmm.
Part of the restructuring I would do it if I were doing this again would be to set ItemData up as a
property with custom accessors that directly access the list. This would make much of the code far
cleaner.
I still really don't understand the purpose of Enums within C#, as the requirement to have them fully
qualified seems to detract from the cleanliness that would otherwise be the purpose of them.
Some features I would have added in the future include:
	The ability to heal - by potions and by a healer in town.
	The ability for enemies to equip items, as currently generated ones just punch things.
	The implementation of other GameBoards via dungeon entrances, etc.
But that's for another time - if ever.