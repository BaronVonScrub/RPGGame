using System;
using System.Collections.Generic;
using System.Text;

namespace RPGGame
{
    class GameBoard
    {
        public Dictionary<Coordinate, List<Entity>> entityPos = new Dictionary<Coordinate, List<Entity>>();
        public static int padding = 15;

        private char LeftTopCornerBorder = (char)9556;
        private char HorizontalBorder = (char)9552;
        private char RightTopCornerBorder = (char)9559;
        private char VerticalBorder = (char)9553;
        private char LeftBottomCornerBorder = (char)9562;
        private char RightBottomCornerBorder = (char)9565;

        View currentView;

        public static int viewDistanceWidth = 5;
        public static int viewDistanceHeight = 5;

        public GameBoard() { }

        public void RenderBoard()
        {
            currentView = EntityManager.player.GetView();

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
            for (int yy= currentView.topLeft.y; yy<currentView.bottomRight.y+1;yy++)
            {
                Pad();
                Draw(VerticalBorder);
                for (int xx=currentView.topLeft.x;xx<currentView.bottomRight.x+1; xx++)
                {
                    Draw((char)32);
                    Draw(GetHighestDrawPriority(GetFromBoard(new Coordinate(xx,yy))));
                }
                Draw((char)32);
                Draw(VerticalBorder);
                Console.WriteLine();
            }
            #endregion

            #region Draw bottom border
            Pad();
            Draw(LeftBottomCornerBorder);
            for (int i = currentView.topLeft.x; i < currentView.bottomRight.x+1; i++)
            {
                Draw(HorizontalBorder);
                Draw(HorizontalBorder);
            }
            Draw(HorizontalBorder);
            Draw(RightBottomCornerBorder);
            Console.WriteLine();
            #endregion
        }

        public static void Pad()
        {
            Console.SetCursorPosition(padding,Console.CursorTop);
        }

        private void Draw(char ch)
        {
            TextTool.Write(ch+"");
        }

        public void AddToBoard(Entity toAdd) {
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
            char ic = (char)2281;
            if (inList!=null)
                foreach (Entity ent in inList)
                    if (ent.drawPriority > highest) {
                        highest = ent.drawPriority;
                        ic = ent.icon;
                    }
            return ic;
        }
    }
}

struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}