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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < pages.Count + 1; i++)
            {
                PdfDocument document = new PdfDocument();
                if (i == 0)
                {
                    document = new PdfDocument();
                }
                else
                {
                    document = CutPDFToMemory("temp.pdf");
                }
                
                if (i < pages.Count)
                    AddImagePage(document, pages[i]);


                Console.Title = ($"{dirName} | {i}/{pages.Count} Pages Done | [" + (int)Math.Round((double)(100 * i) / pages.Count) + "%] | "  + (int)stopwatch.ElapsedMilliseconds / 1000 + " seconds elapsed");
                if (i == pages.Count)
                {
                    document.Save($"{dirName}.pdf");
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("==========");
                    Console.WriteLine("HakuNeko Manga Chapters To PDF // Made by Kye#5000 | https://github.com/kyeondiscord");
                    Console.WriteLine("");
                    Console.WriteLine($"Exported to {dirName}.pdf !");
                    Console.WriteLine("==========");

                    Console.ReadLine();
                }
                else 
                    document.Save($"temp.pdf");
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
                PdfDocument PDFDoc = PdfReader.Open(filename, PdfDocumentOpenMode.Modify);
                PdfDocument PDFNewDoc = PDFDoc;
                PDFDoc.Dispose();
                File.Delete(filename);
                return PDFNewDoc;
            }
            else
            {
                throw new Exception($"{filename} doesn't exist!");
            }
                
        }

    }
}
