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

        public static void ImportEntities()
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\Entities.dat";
            String[] lines = File.ReadAllLines(storageFile);

            for (int lineNum=0; lineNum<lines.Length-1;lineNum+=7)
            {
                String[] entData = { lines[lineNum], lines[lineNum + 1], lines[lineNum + 2], lines[lineNum + 3], lines[lineNum + 4], lines[lineNum + 5] };
                MainBoard.AddToBoard(EntityFactory(entData));
            }
        }

        public static void ExportEntities()
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\Entities.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            foreach (KeyValuePair< Coordinate,List <Entity>> pos in MainBoard.entityPos)
            {
                foreach (Entity ent in pos.Value)
                {
                    sw.WriteLine(ent.Name);
                    sw.WriteLine(ent.GetType().Name);
                    sw.WriteLine(Convert.ToInt32(ent.icon).ToString());
                    sw.WriteLine(ent.drawPriority);
                    sw.WriteLine(ent.position.x.ToString()+" "+ ent.position.y.ToString());
                    if (ent.inventory != null)
                        sw.WriteLine(ent.inventory.name);
                    else
                        sw.WriteLine("Null");
                    sw.WriteLine();
                }
            }
        }

    }
}
