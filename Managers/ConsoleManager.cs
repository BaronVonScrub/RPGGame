using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;
namespace RPGGame
{
    /* Much of this class is derived from the work done by user Jaykul of Github. https://gist.github.com/Jaykul/af95aeece9c3e49815a266bcd8594f1f
    It was initialize just here to help me set the font, but expanded into having other uses.
    I had never seen/used DLL importing in C# prior to this, so it was a great learning experience!*/
    public static class ConsoleManager
    {
        /*********************************************************************************************************************************************************/
        //This first section is largely based on Jaykul's work, thought some additional DLL's and functions were added.
        #region Primarily Jaykul's code
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll")]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        public enum SystemMetric
        {
            VirtualScreenWidth = 78, // CXVIRTUALSCREEN 0x0000004E 
            VirtualScreenHeight = 79, // CYVIRTUALSCREEN 0x0000004F 
        }
        
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric metric);
        #endregion
        /*********************************************************************************************************************************************************/
        #region My code
        /*********************************************************************************************************************************************************/

        //MY CODE This method initalizes everything needed by the console for use in the game
        public static void Initialize()
        {
            ExternalTesting = IsExternalTestMode();                                                 //Checks if the program is running inside of unit tests

            SetUpConsole();                                                                         //Runs Jaykul's setup method

            if (!ExternalTesting)                                                                   //Ignored if in unit tests, as it hangs forever
                SetCurrentFont("Courier New", 25);                                                      //Sets font

            CenterScreen(GetVirtualDisplaySize());                                                  //Centers the console window (I think. Only tested it on my monitor.)

            double minSize = 0;                                                                     //Preps variable as double so I don't need to cast it later.
            if (!ExternalTesting)                                                                   //Ignored if in unit tests, as it hangs forever
                minSize = Console.WindowLeft + Console.WindowWidth;

            if (!ExternalTesting)                                                                   //Ignored if in unit tests, as it hangs forever
                Console.SetBufferSize((int)Math.Ceiling(minSize), 30);                                  //Set the buffer size to to the minimum allowed by resolution
            Console.Title = "RPG GAME";                                                             //Set the title of the window

            IntPtr handle = GetConsoleWindow();                                                     //Set a handle pointer
            IntPtr sysMenu = GetSystemMenu(handle, false);                                          //Gets the system menu from the console window
            if (handle != IntPtr.Zero)                                                              //If there is a valid pointer
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);                                     //Remove the maximize command
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);                                         //Remove the ability to resize
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;                                     //Sets the encoding to UTF8
            Console.ForegroundColor = ConsoleColor.White;                                           //Sets the foreground colour to white
            if (!ExternalTesting)                                                                   //Ignored if in unit tests, as it hangs forever
                Console.CursorVisible = false;                                                          //Hide the cursor
        }

        //MY CODE Gets and returns the screen width and height. 
        public static Size GetVirtualDisplaySize()
        {
            int width = GetSystemMetrics(SystemMetric.VirtualScreenWidth);                          //Gets screen width
            int height = GetSystemMetrics(SystemMetric.VirtualScreenHeight);                        //Gets screen height

            return new Size(width, height);                                                         //Returns the values as a size
        }

        //MY CODE This attempts to center the console in the middle of the screen. Does it do it on other monitors? No idea.
        public static void CenterScreen(Size size) => MoveWindow(ConsoleManager.GetConsoleWindow(), (int)Math.Round(size.Width / 8f), (int)Math.Round(size.Height / 10f), 800, 800, true);

        //MY CODE This function checks if we are running in unit testing mode by scouting the assemblies' names for "unittesting" - the name of my unit testing project.
        public static bool IsExternalTestMode() {
            return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().StartsWith("microsoft.visualstudio.testplatform"));
        }

        /* MY CODE: This handles the rendering process. If the inventory is visible, the map is drawn. The text is then rewritten. This could be rewritten to
         only redraw the map when something on it changes, and then bypassing the map by setting the Cursor position.*/
        public static void Redraw()
        {
            if (!InventoryView && MapDraw)                                                    //Skip the map drawing if you're in an inventory, or if you don't have a change request
            {
                if (!ExternalTesting)                                                           //Ignored if in unit tests, as it hangs forever
                    Console.Clear();                                                            //Clear the console
                MainBoard.RenderBoard();                                                        //Render the main board
                MapDraw = false;                                                              //Reset the change request trigger
            }
            RenderText();                                                                       //Render the text output
        }
        #endregion
        /*********************************************************************************************************************************************************/
        /*From here. this is almost entirely Jaykul's code, but with escapes added for internal and external testing*/
        #region Primarily Jaykul's code
        internal static void SetUpConsole()
        {
            IntPtr iStdOut = ConsoleManager.GetStdHandle(STD_OUTPUT_HANDLE);
            if (!ConsoleManager.GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                Console.Write("failed to get output console mode");
                if (!ExternalTesting&& !InternalTesting)
                    Console.ReadKey();
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!ConsoleManager.SetConsoleMode(iStdOut, outConsoleMode))
            {
                Console.Write($"failed to set output console mode, error code: {ConsoleManager.GetLastError()}");
                if (!ExternalTesting && !InternalTesting)
                    Console.ReadKey();
                return;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]

            public string FontName;
        }

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
        {
            var before = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {

                var set = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };


                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    int ex = Marshal.GetLastWin32Error();
                    Console.Write("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                var after = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            }
            else
            {
                int er = Marshal.GetLastWin32Error();
                Console.Write("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
        #endregion
        /*********************************************************************************************************************************************************/
    }
}