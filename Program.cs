using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Images2Pdf
{
    class Program
    {
        static void Main()
        {
            Console.Title = "HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==========");
            Console.WriteLine("HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord");
            Console.WriteLine("==========");
            Console.ForegroundColor = ConsoleColor.Green;
            string currentdirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dirName = new DirectoryInfo(currentdirectory).Name;
            string[] images = Directory.GetFiles(currentdirectory, "*.jpg*", SearchOption.AllDirectories);
            List<string> pages = new List<string>(images);

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
            PdfDocument document = new PdfDocument();
            int done = 0;
            foreach (string filename in pages)
            {
                    PdfPage page = document.AddPage();
                   
                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    Bitmap img = new Bitmap(filename);
                    int imageHeight = img.Height;
                    int imageWidth = img.Width;
                    DrawImage(gfx, filename, 0, 0, imageHeight / 2, imageWidth / 2);
                    done++;
                    Console.Title = ($"{dirName} | {done}/{pages.Count} Pages done!");
                    
            }
            document.Save($"{dirName}.pdf");
            Console.WriteLine($"Exported to {dirName}");
            Console.ReadLine();
        }
        public static void DrawImage(XGraphics gfx, string image, int x, int y, int height, int width)
        {
            gfx.DrawImage(XImage.FromFile(image), x, y, width, height);
        }

    }
}
