using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace ImageGenerator
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Enter number of pages you want to make:");
            Font font = new Font("Times New Roman", 12.0f);

            if (!Directory.Exists("Chapter 1"))
                Directory.CreateDirectory("Chapter 1");


            int pages = int.Parse(Console.ReadLine());
            for (int i = 0; i < pages; i++)
            {
                DrawText($"Page {i}", font, Color.Black, Color.White).Save(Path.Combine("Chapter 1", i + ".jpg"), ImageFormat.Jpeg);
            }
            Console.WriteLine("Done!");
            Console.ReadKey();

        }



        private static Image DrawText(string text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            //SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            //img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            img = new Bitmap(500,500);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }
    }
}
