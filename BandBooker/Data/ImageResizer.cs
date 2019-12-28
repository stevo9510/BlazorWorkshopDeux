using LazZiya.ImageResize;
using System;
using System.Drawing;

namespace BandBooker.Data
{
    public class ImageResizer
    {
        public static void ResizeAndSaveImage(string inputFile, int maxWidth)
        {
            var img = Image.FromFile(inputFile);
            var extension = System.IO.Path.GetExtension(inputFile);

            string tempfile = Environment.CurrentDirectory
                + "\\" + DateTime.Now.Ticks.ToString()
                + extension;

            int maxHeight = img.Height;

            if (maxWidth == 0)
                maxWidth = img.Width;

            var ratioX = (double)maxWidth / img.Width;
            var ratioY = (double)maxHeight / img.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(img.Width * ratio);
            var newHeight = (int)(img.Height * ratio);

            //resize the image to 600x400 
            var newImg = ImageResize.Scale(img, newWidth, newHeight);

            //save new image
            newImg.SaveAs(tempfile);

            //dispose to free up memory
            img.Dispose();
            newImg.Dispose();

            // delete original file
            System.IO.File.Delete(inputFile);

            // rename tempfile
            System.IO.File.Move(tempfile, inputFile);
        }
    }
}
