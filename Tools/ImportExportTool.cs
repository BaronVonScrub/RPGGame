using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;
using static RPGGame.ParseTool;

namespace RPGGame
{
    class ImportExportTool
    {
        public static void ImportInventories()
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\Inventories.dat";
            String[] lines = File.ReadAllLines(storageFile);
            int lineNum = 0;
            Inventory inv;
            do
            {
                lineNum += 1;
                inv = new Inventory(lines[lineNum]);
                do
                {
                    lineNum += 1;
                    inv.inventData.Add(ItemMake(lines[lineNum]));
                }
                while (lines[lineNum+1] != "" && lines[lineNum + 1] != "ENDFILE");
                lineNum += 1;
                Inventories.Add(inv);
                TextTool.WriteLine(inv.name + " added!");
            }
            while (lineNum != lines.Length - 1);
        }

        public static void ExportInventories()                              
        {
            #region Set paths
            string storageFile = Directory.GetCurrentDirectory()+ "\\Inventories.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            #endregion

            foreach (Inventory inv in Inventories) {
                sw.Write(System.Environment.NewLine);
                sw.WriteLine(inv.name);
                foreach (Item item in inv.inventData)
                {
                    sw.Write(item.itemData["type"].ToUpper());
                    foreach (KeyValuePair<String, String> att in item.itemData)
                        sw.Write(" " + att.Key + ":" + att.Value);
                    sw.Write(System.Environment.NewLine);
                }
            }
            sw.Write("ENDFILE");
            sw.Close();
        }
    }
}
