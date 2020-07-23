using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;

namespace RPGGame
{
    class ImportExportTool
    {
        public static string[] GetInventoryList()                                                        
        {
            string currDir = Directory.GetCurrentDirectory();
            string storageDir = currDir + "\\Inventories";
            Directory.CreateDirectory(storageDir);
            string[] dirList = Directory.GetFiles(storageDir);
            string[] nameClean = dirList.Select(x => x.Replace(storageDir + "\\", "")).ToArray();
            string[] nameClean2 = nameClean.Select(x => x.Replace(".dat", "")).ToArray();
            return nameClean2;
        }

        public static void importInventory(string inventoryName)                                                    
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
            Inventory inv = GetInventory(inventoryName);
            foreach (string item in File.ReadLines(inventFile))                                              
                inv.inventData.Add(ParseTool.ItemMake(item));                                                         
            Inventories.Add(inv);

            #endregion
        }

        public static void ExportInventory(Inventory inventory)                              
        {
            #region Set paths
            string currDir = Directory.GetCurrentDirectory();
            string storageDir = currDir + "\\Inventories";
            string invent = inventory.name;
            string inventFile = storageDir + "\\" + invent + ".dat";
            #endregion

            #region Output to file
            File.WriteAllText(inventFile, "");                                                               
            using (StreamWriter sw = File.CreateText(inventFile))
            {
                foreach (Item item in inventory.inventData)
                {                                                     
                    sw.Write(item.itemData["type"].ToUpper());                                               
                    foreach (KeyValuePair<String, String> att in item.itemData)                              
                    {
                        sw.Write(" " + att.Key + ":" + att.Value);
                    }
                    sw.Write(System.Environment.NewLine);                                                    
                }
            }


            #endregion
        }
    }
}
