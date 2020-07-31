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
        public static void ImportInventories(string filename)
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\"+filename;
            string[] lines = File.ReadAllLines(storageFile);
            Inventory inv = null;
            foreach (string data in lines)
            {
                switch (data)
                {
                    case var someVal when (new Regex("^[\\w\\s]+$").IsMatch(someVal)):                  //No attributes, but not empty
                        inv = new Inventory(data, new List<Item>());
                        break;
                    case var someVal when (new Regex(AttFinder).Matches(someVal).Count != 0):           //There are attributes
                        Item item = ItemCreate(new ItemData(data));
                        inv.inventData.Add(item);
                        break;
                    default:                                                                            //Empty
                        Inventories.Add(inv);
                        break;
                }
            }
        }

        public static void ExportInventories()
        {
            #region Set paths
            string storageFile = Directory.GetCurrentDirectory() + "\\Inventories.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            #endregion

            foreach (Inventory inv in Inventories)
            {
                sw.WriteLine(inv.name);
                foreach (Item item in inv.inventData)
                {
                    sw.Write(item.GetType().Name.ToUpper());
                    foreach (KeyValuePair<string, string> att in item.itemData)
                        sw.Write(" " + att.Key + ":" + att.Value);
                    sw.Write(System.Environment.NewLine);
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public static void ImportEntities(string filename)
        {
            MainBoard = new GameBoard();

            string storageFile = Directory.GetCurrentDirectory() + "\\"+filename;
            string[] lines = File.ReadAllLines(storageFile);

            for (int lineNum = 0; lineNum < lines.Length - 1; lineNum += 9)
            {
                string[] entData = new string[8];
                Array.Copy(lines, lineNum, entData, 0, 8);
                MainBoard.AddToBoard(EntityCreate(new EntityData(entData)));
            }

            Player = GetEntity("Player");
        }

        public static void ExportEntities()
        {
            string storageFile = Directory.GetCurrentDirectory() + "\\Entities.dat";
            File.WriteAllText(storageFile, "");
            using StreamWriter sw = File.CreateText(storageFile);
            foreach (KeyValuePair<Coordinate, List<Entity>> pos in MainBoard.entityPos)
            {
                foreach (Entity ent in pos.Value)
                {
                    sw.WriteLine(ent.Name);
                    sw.WriteLine(ent.GetType().Name);
                    sw.WriteLine(Convert.ToInt32(ent.icon).ToString());
                    sw.WriteLine(ent.drawPriority);
                    sw.WriteLine(ent.position.x.ToString() + " " + ent.position.y.ToString());
                    if (ent.inventory != null)
                        sw.WriteLine(ent.inventory.name);
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
                }
            }
        }

    }
}