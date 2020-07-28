using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static RPGGame.GlobalVariables;
using static RPGGame.InventoryManager;
using static RPGGame.ParseTool;
using static RPGGame.EntityManager;
using System.Runtime.InteropServices.ComTypes;

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
                if (lineNum==lines.Length)
                    break;

                do
                {
                    if (lineNum == lines.Length)
                        break;
                    if (lines[lineNum] == "")
                        break;
                    lineNum++;
                }
                while (true);

                if (lineNum == lines.Length)
                    break;
                inv = new Inventory(lines[lineNum]);
                do
                {
                    lineNum += 1;
                    if (lineNum == lines.Length)
                        break;
                    if (lines[lineNum] == "")
                        break;
                    ItemData data = new ItemData(lines[lineNum]);
                    inv.inventData.Add(ItemCreate(data));
                }
                while (true);
                lineNum += 1;
                Inventories.Add(inv);
            }
            while (true);
        }

        public static void ExportInventories()                              
        {
            #region Set paths
            string storageFile = Directory.GetCurrentDirectory()+ "\\Inventories.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            #endregion

            foreach (Inventory inv in Inventories) {
                sw.WriteLine(inv.name);
                foreach (Item item in inv.inventData)
                {
                    sw.Write(item.GetType().Name.ToUpper());
                    foreach (KeyValuePair<String, String> att in item.itemData)
                        sw.Write(" " + att.Key + ":" + att.Value);
                    sw.Write(System.Environment.NewLine);
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public static void ImportEntities()
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\Entities.dat";
            String[] lines = File.ReadAllLines(storageFile);

            for (int lineNum=0; lineNum<lines.Length-1;lineNum+=8)
            {
                string[] entData = new string[7];
                Array.Copy(lines, lineNum, entData, 0, 7);
                MainBoard.AddToBoard(EntityCreate(new EntityData(entData)));
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
                    for(int i= 0; i< ent.Stats.Length;i++)
                    {
                        sw.Write(ent.Stats[i]);
                        if (i != ent.Stats.Length - 1)
                            sw.Write(" ");
                    }
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
        }

    }
}
