using System;
using System.Collections.Generic;
using static RPGGame.ConsoleManager;
using static RPGGame.ConstantVariables;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal class GameBoard
    {
        private View currentView;
        public Dictionary<Coordinate, List<Entity>> entityPos = new Dictionary<Coordinate, List<Entity>>();

        public GameBoard() { }

        public void RenderBoard()
        {
            currentView = Player.GetView();
            bool wallBuffer = false;

            #region Draw top border
            Pad();
            Draw(LeftTopCornerBorder);
            for (int i = currentView.topLeft.x; i < currentView.bottomRight.x + 1; i++)
            {
                Draw(HorizontalBorder);
                Draw(HorizontalBorder);
            }
            Draw(HorizontalBorder);
            Draw(RightTopCornerBorder);
            Console.WriteLine();
            #endregion

            #region Draw side borders and map
            for (int yy = currentView.topLeft.y; yy < currentView.bottomRight.y + 1; yy++)
            {
                Pad();
                Draw(VerticalBorder);
                for (int xx = currentView.topLeft.x; xx < currentView.bottomRight.x + 1; xx++)
                {
                    if (wallBuffer == false)
                        Draw((char)32);
                    else
                        wallBuffer = false;

                    char toDraw = GetHighestDrawPriority(GetFromBoard(new Coordinate(xx, yy)));
                    Draw(toDraw);

                    if (toDraw == Hor)
                    {
                        Write("\b\b" + Hor + Hor + Hor);
                        wallBuffer = true;
                    }
                    if (toDraw == TopL)
                    {
                        wallBuffer = true;
                        Draw(Hor);
                    }
                    if (toDraw == TopR)
                    {
                        Write("\b\b" + Hor + TopR);
                    }
                    if (toDraw == BotL)
                    {
                        wallBuffer = true;
                        Draw(Hor);
                    }
                    if (toDraw == BotR)
                    {
                        Write("\b\b" + Hor + BotR);
                    }
                    //
                    if (toDraw == HPath || toDraw == TTPath || toDraw == TBPath || toDraw == XPath)
                    {
                        Write("\b\b" + HPath + toDraw + HPath);
                        wallBuffer = true;
                    }
                    if (toDraw == LTPath)
                    {
                        wallBuffer = true;
                        Draw(HPath);
                    }
                    if (toDraw == RTPath)
                    {
                        Write("\b\b" + HPath + RTPath);
                    }
                    if (toDraw == LBPath)
                    {
                        wallBuffer = true;
                        Draw(Hor);
                    }
                    if (toDraw == RBPath)
                    {
                        Write("\b\b" + HPath + RBPath);
                    }
                }
                Draw((char)32);
                if (wallBuffer == true)
                {
                    Write("\b");
                    wallBuffer = false;
                }
                Draw(VerticalBorder);
                Console.WriteLine();
            }
            #endregion

            #region Draw bottom border
            Pad();
            Draw(LeftBottomCornerBorder);
            for (int i = currentView.topLeft.x; i < currentView.bottomRight.x + 1; i++)
            {
                Draw(HorizontalBorder);
                Draw(HorizontalBorder);
            }
            Draw(HorizontalBorder);
            Draw(RightBottomCornerBorder);
            Console.WriteLine();
            #endregion

            string positionReadOut = "(" + Player.position.x + "," + Player.position.y + ")";
            if (!IsTestMode())
                Console.SetCursorPosition(Console.BufferWidth / 2 - positionReadOut.Length / 2, Console.CursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(positionReadOut);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Pad()
        {
            if (!IsTestMode())
                Console.SetCursorPosition(padding, Console.CursorTop);
        }

        private void Draw(char ch) => Write(ch + "");

        public void AddToBoard(Entity toAdd)
        {
            if (!entityPos.ContainsKey(toAdd.position))
                entityPos.Add(toAdd.position, new List<Entity>());
            entityPos[toAdd.position].Add(toAdd);
        }

        public List<Entity> GetFromBoard(Coordinate pos)
        {
            if (entityPos.ContainsKey(pos))
                return entityPos[pos];
            else
                return null;
        }

        public char GetHighestDrawPriority(List<Entity> inList)
        {
            int highest = 0;
            char ic = (char)1622;
            if (inList != null)
                foreach (Entity ent in inList)
                    if (ent.drawPriority > highest)
                    {
                        highest = ent.drawPriority;
                        ic = ent.icon;
                    }
            return ic;
        }
    }
}