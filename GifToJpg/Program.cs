using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GifToJpg
{
    class Program
    {
        public static void Main(string[] args)
        {
            Image gif = Image.FromFile("test2.gif");
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd);
            for (int i = 0; i < count; i++)
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save("test2_" + string.Format("{0:00}", i) + ".jpg", ImageFormat.Jpeg);
            }
        }
    }
}