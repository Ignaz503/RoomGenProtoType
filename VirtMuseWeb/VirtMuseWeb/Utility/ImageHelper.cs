using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using UnityEngine;


namespace VirtMuseWeb.Utility
{
    public static class ImageHelper
    {
        public static Bitmap GetBitmap(string dataURL)
        {
            var binData = Convert.FromBase64String(dataURL);

            using (MemoryStream st = new MemoryStream(binData))
            {
                return new Bitmap(st);
            }
        }

        public static Bitmap GetBitmap(byte[] imgData)
        {
            using (MemoryStream st = new MemoryStream(imgData))
            {
                return new Bitmap(st);
            }
        }

        public static Texture2D GetImageAsUnityTexture2D(string dataURL)
        {
            return GetImageAsUnityTexture2D(Convert.FromBase64String(dataURL));
        }

        public static Texture2D GetImageAsUnityTexture2D(byte[] imgData)
        {
            Bitmap img = GetBitmap(imgData);
            Texture2D tex = new Texture2D(img.Width, img.Height);
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    System.Drawing.Color c = img.GetPixel(x, y);

                    tex.SetPixel(x, y, new Color32(c.R, c.G, c.B, c.A));

                }
            }
            tex.Apply();
            return tex;
        }

        public static Utility.Image GetImage(byte[] imgData)
        {
            Bitmap map = GetBitmap(imgData);
            Utility.Image img = new Utility.Image(map.Width, map.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    System.Drawing.Color col = map.GetPixel(x, y);
                    img[x, y] = new Utility.Image.Color32(col.A, col.R, col.G, col.B);
                }
            }
            return img;
        }

        public static Utility.Image GetImage(string dataURL)
        {
            return GetImage(Convert.FromBase64String(dataURL));
        }

        public static Utility.Image GetImage(Bitmap map)
        {
            Image img = new Image(map.Width, map.Height);
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    System.Drawing.Color col = map.GetPixel(x, y);
                    img[x, y] = new Utility.Image.Color32(col.A, col.R, col.G, col.B);
                }
            }
            return img;
        }

        /// <summary>
        /// makes all the images to a certain size
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public static Bitmap[] MakeSameSize(Bitmap[] images, (float, float) dimensions)
        {
            foreach(Bitmap map in images)
            {
                Resize(map, dimensions);
            }
            return images;
        }

        public static Bitmap Resize(Bitmap map, (float,float) dimensions)
        {
            var brush = new SolidBrush(System.Drawing.Color.Black);

            float width = dimensions.Item1;
            float height = dimensions.Item2;
            var bmp = new Bitmap((int)width, (int)height);
            float scale = Math.Min(width / map.Width, height / map.Height);
            using (var graph = System.Drawing.Graphics.FromImage(bmp))
            {
                //higher quality output
                graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int scaleWidth = (int)(map.Width * scale);
                int scaleHeight = (int)(map.Height * scale);

                graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
                graph.DrawImage(map, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
                return bmp;
            }
        }
    }
}
