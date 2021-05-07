using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Images2Pdf
{
    class Program
    {
        static void Main()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Console.Title = "HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==========");
            Console.WriteLine("HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord");
            Console.WriteLine("==========");
            Console.ForegroundColor = ConsoleColor.Green;
            string currentdirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dirName = new DirectoryInfo(currentdirectory).Name;
            string[] a = Directory.GetFiles(currentdirectory, "*.jpg*", SearchOption.AllDirectories);
            Array.Sort(a, new WindowsFileNames());
            List<string> pages = new List<string>(a);

            if (pages.Count == 0)
            {
                Console.WriteLine("Put this in the folder HakuNeko generates with all the chapters!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Generating {dirName}.pdf");
            Console.Title = $"{dirName} | Initializing...";

            Console.WriteLine($"Found {pages.Count} pages. Press any key to continue...");


            Console.ReadKey();
            Console.Clear();
            Console.WriteLine($"Creating {dirName}.pdf ...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            PdfDocument document = new PdfDocument();
            for (int i = 0; i < pages.Count; i ++)
            {
                float mem = CurrentMemorySize;
                AddImagePage(document, pages[i]);
                Console.WriteLine("Added " + i + "   | Memory: " + mem);
                if (mem > 2048)//2GB of Ram
                {
                    document.Save("tmp.pdf");
                    document = new PdfDocument();
                    document = CutPDFToMemory("tmp.pdf");
                }
            Console.Title = ($"{dirName} | {i}/{pages.Count} Pages Done | [" + (int)Math.Round((double)(100 * i) / pages.Count) + "%] | " + (int)stopwatch.ElapsedMilliseconds / 1000 + " seconds elapsed");
            }
            document.Save($"{dirName}.pdf");
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==========");
            Console.WriteLine("HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord");
            Console.WriteLine("");
            Console.WriteLine($"Exported to {dirName}.pdf!");
            Console.WriteLine("==========");
            DrawImage();

            Console.WriteLine($"Enjoy your manga :3");
            File.Delete("tmp.pdf");
            Console.ReadLine();
        }
        public static void AddImagePage(PdfDocument document, string filename)
        {
            PdfPage page = document.AddPage();
            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            Bitmap img = new Bitmap(filename);
            int imageHeight = img.Height;
            int imageWidth = img.Width;
            page.Width = imageWidth;
            page.Height = imageHeight;
            gfx.DrawImage(XImage.FromFile(filename), 0, 0, imageWidth,imageHeight );
        }
        public static float CurrentMemorySize
        {
            get
            {
                //ConvertBytesToMegabytes
                return (Process.GetCurrentProcess().PrivateMemorySize64 / 1024f) / 1024f;
            }
        }
        

        public static PdfDocument CutPDFToMemory(string filename)
        {
            
                return PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
        }

        public static void DrawImage()
        {
            Point location = new Point(0, 10);
            Size imageSize = new Size(16, 8); // desired image size in characters
            using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
            {
                using (Image image = Properties.Resources.dango)
                {
                    Size fontSize = GetConsoleFontSize();

                    // translating the character positions to pixels
                    Rectangle imageRect = new Rectangle(
                        location.X * fontSize.Width,
                        location.Y * fontSize.Height,
                        imageSize.Width * fontSize.Width,
                        imageSize.Height * fontSize.Height);
                    g.DrawImage(image, imageRect);
                }
            }
        }



        private static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            IntPtr outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            int errorCode = Marshal.GetLastWin32Error();
            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();
            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            [Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential)]
        internal class ConsoleFontInfo
        {
            internal int nFont;
            internal Coord dwFontSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Coord
        {
            [FieldOffset(0)]
            internal short X;
            [FieldOffset(2)]
            internal short Y;
        }

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int OPEN_EXISTING = 3;


        public class WindowsFileNames : IComparer<string>
        {

            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            static extern int StrCmpLogicalW(String x, String y);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }
        }
    }
}
