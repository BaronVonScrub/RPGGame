using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RPGGame
{
    class ImportExportTool
    {
        public static string[] GetInventoryList()                                                        //Gets a list of all InventoryManager.inventories from an index file
        {
            string currDir = Directory.GetCurrentDirectory();
            string storageDir = currDir + "\\Inventories";
            Directory.CreateDirectory(storageDir);
            string[] dirList = Directory.GetFiles(storageDir);
            string[] nameClean = dirList.Select(x => x.Replace(storageDir + "\\", "")).ToArray();
            string[] nameClean2 = nameClean.Select(x => x.Replace(".dat", "")).ToArray();
            return nameClean2;
        }

        public static void importInventory(string inventoryName)                                                    //Imports an inventory from file
        {
            #region Set paths
            string currDir = Directory.GetCurrentDirectory();
            string storageDir = currDir + "\\Inventories";
            string inventFile = storageDir + "\\" + inventoryName + ".dat";
            #endregion

            #region Ensure file exists
            if (!File.Exists(inventFile))
                using (StreamWriter sw = File.CreateText(inventFile))
                    sw.Close();
            #endregion

            #region Read File
            Inventory inv = InventoryManager.GetInventory(inventoryName);
            foreach (string item in File.ReadLines(inventFile))                                              //For each line(item) in the inventory                                                                                           //Get the inventory name from the file name
                inv.inventData.Add(ParseTool.ItemMake(item));                                                         //Uses the add command to create each loaded file in inventory
            InventoryManager.inventories.Add(inv);

            #endregion
        }

        public static void ExportInventory(Inventory inventory)                              //Exports an inventory to file 
        {
            #region Set paths
            string currDir = Directory.GetCurrentDirectory();
            string storageDir = currDir + "\\Inventories";
            string inventName = inventory.name;
            string inventFile = storageDir + "\\" + inventName + ".dat";
            #endregion

            #region Output to file
            File.WriteAllText(inventFile, "");                                                               //Blank out file
            using (StreamWriter sw = File.CreateText(inventFile))
            {
                foreach (Item item in inventory.inventData)
                {                                                     //For each item in the inventory
                    sw.Write(item.itemData["type"].ToUpper());                                               //MERCHANT its type first as a command, for import processing purposes
                    foreach (KeyValuePair<String, String> att in item.itemData)                              //MERCHANT all of its data in the proper format
                    {
                        sw.Write(" " + att.Key + ":" + att.Value);
                    }
                    sw.Write(System.Environment.NewLine);                                                    //Next line + item
                }
            }


            #endregion
        }
    }
}
