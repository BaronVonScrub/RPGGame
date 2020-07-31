using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static RPGGame.GlobalVariables;
using static RPGGame.TextManager;
namespace RPGGame
{
    public static class ConsoleManager
    {
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

        public enum SystemMetric
        {
            VirtualScreenWidth = 78, // CXVIRTUALSCREEN 0x0000004E 
            VirtualScreenHeight = 79, // CYVIRTUALSCREEN 0x0000004F 
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric metric);

        public static Size GetVirtualDisplaySize()
        {
            int width = GetSystemMetrics(SystemMetric.VirtualScreenWidth);
            int height = GetSystemMetrics(SystemMetric.VirtualScreenHeight);

            return new Size(width, height);
        }

        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        public static void CenterScreen(Size size) => MoveWindow(ConsoleManager.GetConsoleWindow(), (int)Math.Round(size.Width / 8f), (int)Math.Round(size.Height / 10f), 800, 800, true);

        public static void Initialize()
        {
            ExternalTesting = IsExternalTestMode();
            SetUpConsole();
            if (!ExternalTesting)
                SetCurrentFont("Courier New", 25);
            CenterScreen(GetVirtualDisplaySize());
            Console.WriteLine();
            double minSize = 0;
            if (!ExternalTesting)
                minSize = Console.WindowLeft + Console.WindowWidth;
            if (!ExternalTesting)
                Console.SetBufferSize((int)Math.Ceiling(minSize), 30);
            Console.Title = "RPG GAME";

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.White;
            if (!ExternalTesting)
                Console.CursorVisible = false;
        }

        public static bool IsExternalTestMode() => AppDomain.CurrentDomain.GetAssemblies().Any(
                a => a.FullName.ToLowerInvariant().StartsWith("unittesting"));

        public static bool InternalTestMode() => InternalTesting;

        public static void Redraw()
        {
            if (!InventoryView)
            {
                if (!ExternalTesting)
                    Console.Clear();
                MainBoard.RenderBoard();
            }
            RenderText();
        }

        internal static void SetUpConsole()
        {
            IntPtr iStdOut = ConsoleManager.GetStdHandle(STD_OUTPUT_HANDLE);
            if (!ConsoleManager.GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                Console.WriteLine("failed to get output console mode");
                if (!ExternalTesting&& !InternalTestMode())
                    Console.ReadKey();
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!ConsoleManager.SetConsoleMode(iStdOut, outConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {ConsoleManager.GetLastError()}");
                if (!ExternalTesting && !InternalTestMode())
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
                    Console.WriteLine("Set error " + ex);
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
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
    }
}