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

    }
}
