using System;
using System.Collections.Generic;
using static RPGGame.ConsoleManager;
using static RPGGame.GlobalConstants;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;

namespace RPGGame
{
    internal class GameBoard
    {
        private View currentView;                                                                                   //Stores the current view in mapspace of the board
        public Dictionary<Coordinate, List<Entity>> entityPos = new Dictionary<Coordinate, List<Entity>>();         //Stores all of the entities in each space by coordinate. This lets the map be bound
                                                                                                                                        //only by the integer limit.
        public GameBoard() { MapDraw = true; }                                                                      //Constructor calls for an inital mapdraw.

        //This method draws the board to the console.
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
            for (int yy = currentView.topLeft.y; yy < currentView.bottomRight.y + 1; yy++)                          //Setting up vertical array to draw borders and coordinates.
            {
                Pad();                                                                                              //Adds an amount of spacing, to center the map on the screen.
                Draw(VerticalBorder);                                                                               //Draws the vertical border character on the left
                for (int xx = currentView.topLeft.x; xx < currentView.bottomRight.x + 1; xx++)                      //Sets up the horizontal array to draw borders and coordinates
                {
                    //This region corrects the offset made by the previous character if it were a wall or path, since they are connected accross the horizontal spacing squares
                    #region Wall buffering
                    if (wallBuffer == false)
                        Draw((char)32);
                    else
                        wallBuffer = false;
                    #endregion

                    char toDraw = GetHighestDrawPriority(GetFromBoard(new Coordinate(xx, yy)));                     //Get the character of the entity with the highest draw priority from the coordinate
                    Draw(toDraw);                                                                                   //Draws that character

                    //This region connects wall and path characters accross the normal horizontal spacing
                    #region Spacing connect
                    switch (toDraw)
                    {
                        #region Walls
                        case Hor:
                        case TeeU:
                        case TeeD:
                        case Cross:
                            Write("\b\b" + Hor + Hor + Hor);
                            wallBuffer = true;
                            break;
                        case TopL:
                        case BotL:
                            wallBuffer = true;
                            Draw(Hor);
                            break;
                        case TopR:
                        case BotR:
                            Write("\b\b" + Hor + toDraw);
                            break;
                        #endregion

                        #region Paths
                        case HPath:
                        case TTPath:
                        case TBPath:
                        case XPath:
                            Write("\b\b" + HPath + toDraw + HPath);
                            wallBuffer = true;
                            break;
                        case LTPath:
                        case LBPath:
                            wallBuffer = true;
                            Draw(HPath);
                            break;
                        case RTPath:
                        case RBPath:
                            Write("\b\b" + HPath + RTPath);
                            break;
                        default:
                            break;
                            #endregion
                    }
                    #endregion
                }

                #region More Wall buffering
                if (wallBuffer == false)
                    Draw((char)32);
                else
                    wallBuffer = false;
                #endregion

                Draw(VerticalBorder);                                                                               //Draw the vertical border character on the right
                Console.WriteLine();                                                                                //Next line
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

            string positionReadOut = "(" + Player.position.x + "," + Player.position.y + ")";                       //Set up a coordinate string
            if (!ExternalTesting)                                                                                   //Ignore if external testing (crashes due to no console)
                Console.SetCursorPosition(Console.BufferWidth / 2 - positionReadOut.Length / 2, Console.CursorTop); //Position the cursor in the center
            Console.ForegroundColor = ConsoleColor.Green;                                                           //Color the text green
            Console.WriteLine(positionReadOut);                                                                     //Write the coordinate string
            Console.ForegroundColor = ConsoleColor.White;                                                           //Restore the text to white
        }

        //This method applies the necessary offset to a line of the map
        public static void Pad()
        {
            if (!ExternalTesting)
                Console.SetCursorPosition(padding, Console.CursorTop);
        }

        //Writes the given character (in green!)
        private void Draw(char ch) => Write(ch + "");

        //Adds the provided entity to the dictionary, providing a new coordinate reference if it lacks one.
        public void AddToBoard(Entity toAdd)
        {
            if (!entityPos.ContainsKey(toAdd.position))
                entityPos.Add(toAdd.position, new List<Entity>());
            entityPos[toAdd.position].Add(toAdd);
        }

        //Provides the list of entities at a given coordinate, if any.
        public List<Entity> GetFromBoard(Coordinate pos)
        {
            if (entityPos.ContainsKey(pos))
                return entityPos[pos];
            else
                return null;
        }

        //Gets the highest draw priority character from entities in a given list, defaulting to a character
        public char GetHighestDrawPriority(List<Entity> inList)
        {
            int highest = 0;
            char ic = (char)1622;                                                                                   //This is the default character
            if (inList != null)
                foreach (Entity ent in inList)
                    if (ent.DrawPriority > highest)
                    {
                        highest = ent.DrawPriority;
                        ic = ent.Icon;
                    }
            return ic;
        }
    }
}