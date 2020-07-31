using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static RPGGame.GlobalConstants;
using static RPGGame.EntityManager;
using static RPGGame.GlobalVariables;
using static RPGGame.ParseManager;

namespace RPGGame
{
    internal class ImportExportManager
    {
        //Imports inventories from file given filename
        public static void ImportInventories(string filename)
        {
            #region Setup
            string storageFile = Directory.GetCurrentDirectory() + "\\"+filename;
            string[] lines = File.ReadAllLines(storageFile);
            Inventory inv = null;
            #endregion

            foreach (string data in lines)                                                              //For each line
            {
                switch (data)
                {
                    case var someVal when (new Regex("^[\\w\\s]+$").IsMatch(someVal)):                  //If it has no attributes, but isn't empty
                        inv = new Inventory(data, new List<Item>());                                    //It's the name of a new inventory, so prep one.
                        break;
                    case var someVal when (new Regex(AttFinder).Matches(someVal).Count != 0):           //There are attributes
                        Item item = ItemCreate(new ItemData(data));                                     //Create an item from the data on this line
                        inv.inventData.Add(item);                                                       //Add it to the current inventory.
                        break;
                    default:                                                                            //It is empty, this is the end of an inventory
                        Inventories.Add(inv);                                                           //So add it to the list of inventories
                        break;
                }
            }
        }

        //Exports the inventories to file
        public static void ExportInventories()
        {
            #region Setup
            string storageFile = Directory.GetCurrentDirectory() + "\\Inventories.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            #endregion

            foreach (Inventory inv in Inventories)                                                      //For each inventory
            {
                sw.WriteLine(inv.name);
                foreach (Item item in inv.inventData)                                                   //For each item in the inventory
                {
                    sw.Write(item.GetType().Name.ToUpper());                                            //Write the item type in all caps
                    foreach (KeyValuePair<string, string> att in item.itemData)                         //For each attribute
                        sw.Write(" " + att.Key + ":" + att.Value);                                      //Store it in the appropriate format
                    sw.Write(System.Environment.NewLine);                                               //Newline
                }
                sw.WriteLine();                                                                         //Newline
            }
            sw.Close();
        }

        //Imports the entities given a file name
        public static void ImportEntities(string filename)
        {
            #region Setup
            MainBoard = new GameBoard();
            string storageFile = Directory.GetCurrentDirectory() + "\\"+filename;
            string[] lines = File.ReadAllLines(storageFile);
            #endregion

            for (int lineNum = 0; lineNum < lines.Length - 1; lineNum += 9)                             //For each set of entity data (9 lines)
            {
                string[] entData = new string[8];                                                       //Prepary 8 slots for data
                Array.Copy(lines, lineNum, entData, 0, 8);                                              //Copy the next 8 lines into data
                MainBoard.AddToBoard(EntityCreate(new EntityData(entData)));                            //Crete an entity from that data and add it to the board
            }

            Player = GetEntity("Player");
        }

        //Exports the entities to file
        public static void ExportEntities()
        {
            #region Setup
            string storageFile = Directory.GetCurrentDirectory() + "\\Entities.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            #endregion

            foreach (KeyValuePair<Coordinate, List<Entity>> pos in MainBoard.entityPos)                 //For each coordinate
            {
                foreach (Entity ent in pos.Value)                                                       //For each entity at that coordinate
                {
                    #region Write the data to file
                    sw.WriteLine(ent.Name);
                    sw.WriteLine(ent.GetType().Name);
                    sw.WriteLine(Convert.ToInt32(ent.Icon).ToString());
                    sw.WriteLine(ent.DrawPriority);
                    sw.WriteLine(ent.position.x.ToString() + " " + ent.position.y.ToString());
                    if (ent.Inventory != null)
                        sw.WriteLine(ent.Inventory.name);
                    else
                        sw.WriteLine("Null");
                    for (int i = 0; i < ent.Stats.Length; i++)
                    {
                        sw.Write(ent.Stats[i]);
                        if (i != ent.Stats.Length - 1)
                            sw.Write(" ");
                    }
                    sw.WriteLine();
                    sw.WriteLine(ent.Description);
                    sw.WriteLine();
                    #endregion
                }
            }
        }

    }
}