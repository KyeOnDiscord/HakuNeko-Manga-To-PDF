using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            string[] pages = Directory.GetFiles(currentdirectory, "*.jpg*", SearchOption.AllDirectories);
            //string[] pages = images;

            if (pages.Length == 0)
            {
                Console.WriteLine("Put this in the folder HakuNeko generates with all the chapters!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"Generating {dirName}.pdf");
            Console.Title = $"{dirName} | Initializing...";

            Console.WriteLine($"Found {pages.Length} pages. Press any key to continue...");

            Console.ReadKey();
            Console.Clear();
            Console.WriteLine($"Creating {dirName}.pdf ...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < pages.Length;)
            {
                PdfDocument document = new PdfDocument();
                if (i == 0)
                {
                    document = new PdfDocument();
                }
                else
                {
                    document = CutPDFToMemory("tmp.pdf");
                }


                if (Enumerable.Range(pages.Length - 15, pages.Length).Contains(i))
                {
                    AddImagePage(document, pages[i]);
                    i++;
                }
                else
                {
                    AddImagePage(document, pages[i]);
                    AddImagePage(document, pages[i + 1]);
                    AddImagePage(document, pages[i + 2]);
                    AddImagePage(document, pages[i + 3]);
                    AddImagePage(document, pages[i + 4]);
                    AddImagePage(document, pages[i + 5]);
                    AddImagePage(document, pages[i + 7]);
                    i++;
                    i++;
                    i++;
                    i++;
                    i++;
                    i++;
                    i++;
                }
                Console.Title = ($"{dirName} | {i}/{pages.Length} Pages Done | [" + (int)Math.Round((double)(100 * i) / pages.Length) + "%] | " + (int)stopwatch.ElapsedMilliseconds / 1000 + " seconds elapsed");
                if (i == pages.Length)
                {
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
                else
                    document.Save("tmp.pdf");
            }
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


        public static PdfDocument CutPDFToMemory(string filename)
        {
            if (File.Exists(filename))
            {
                return PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
            }
            else
            {
                throw new Exception($"{filename} doesn't exist!");
            }
                
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


    }
}
