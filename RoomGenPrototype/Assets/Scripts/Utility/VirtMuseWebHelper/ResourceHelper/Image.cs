using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// Represents an easily serializable Image resource
    /// that can be cast to a UnityTexture2D
    /// used by server to store image data
    /// </summary>
    [Serializable]
    public class Image : ISerializable
    {
        /// <summary>
        /// Color struct that is serializable
        /// and used by VirtMuseWeb.Utility.Image
        /// to represent color
        /// </summary>
        [Serializable]
        public struct Color32
        {
            /// <summary>
            /// Red
            /// </summary>
            [SerializeField] public byte R { get; set; }
            /// <summary>
            /// Green
            /// </summary>
            [SerializeField] public byte G { get; set; }
            /// <summary>
            /// Blue
            /// </summary>
            [SerializeField] public byte B { get; set; }
            /// <summary>
            /// Alpha
            /// </summary>
            [SerializeField] public byte A { get; set; }
            public Color32(int Argb)
            {
                byte[] bytes = BitConverter.GetBytes(Argb);

                if (BitConverter.IsLittleEndian)
                {
                    A = bytes[3];
                    R = bytes[2];
                    G = bytes[1];
                    B = bytes[0];
                }
                else
                {
                    A = bytes[0];
                    R = bytes[3];
                    G = bytes[2];
                    B = bytes[1];
                }

            }
            public Color32(byte a, byte r, byte g, byte b)
            {
                A = a;
                R = r;
                G = g;
                B = b;
            }

            public static explicit operator int(Color32 c)
            {
                return BitConverter.ToInt32(
                    BitConverter.IsLittleEndian ? new byte[] { c.B, c.G, c.R, c.A } : new byte[] { c.A, c.B, c.G, c.R },
                    0
                    );
            }
            public static implicit operator UnityEngine.Color32(Color32 c)
            {
                return new UnityEngine.Color32(c.R, c.G, c.B, c.A);
            }
            public static Color32 White { get { return new Color32(255, 255, 255, 255); } }
            public static Color32 Black { get { return new Color32(255, 0, 0, 0); } }
        }

        /// <summary>
        /// Image width
        /// </summary>
        [SerializeField]
        public int Width { get; set; }
        /// <summary>
        /// Image height
        /// </summary>
        [SerializeField]
        public int Height { get; set; }
        /// <summary>
        /// Image as a 1D color array
        /// </summary>
        [SerializeField]
        Color32[,] color;

        public Color32 this[int x, int y] { get { return color[x,y]; } set { color[x,y] = value; } }

        public Image(int w, int h)
        {
            Width = w;
            Height = h;
            color = new Color32[Width,Height];
        }

        /// <summary>
        /// serializes image to byte array
        /// </summary>
        /// <returns>image as byte array</returns>
        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(ms))
                {
                    WriteTo(w);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Writes image to binary writer
        /// </summary>
        /// <param name="w">the writer we need to write to</param>
        void WriteTo(BinaryWriter w)
        {
            w.Write((Int32)Width);
            w.Write((Int32)Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color32 c = color[x, y];
                    w.Write(c.A);
                    w.Write(c.R);
                    w.Write(c.G);
                    w.Write(c.B);
                }
            }

        }
        /// <summary>
        /// serializes image into already existing binary writer
        /// </summary>
        /// <param name="w">the writer we need to write to</param>
        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }

        /// <summary>
        /// deserializes Image from byte array
        /// </summary>
        /// <param name="obj">the byte array representing the image</param>
        /// <returns>the Image object created from the byte array</returns>
        public static Image Deserialize(byte[] obj)
        {
            using (MemoryStream ms = new MemoryStream(obj))
            {
                using (BinaryReader r = new BinaryReader
                    (ms))
                {
                    return ReadFrom(r);
                }
            }
        }

        /// <summary>
        /// Reads Image from binary reader
        /// </summary>
        /// <param name="r">the reader we read the object from</param>
        /// <returns>the image object read from the binary reader</returns>
        static Image ReadFrom(BinaryReader r)
        {
            int width = r.ReadInt32();
            int height = r.ReadInt32();
            Image img = new Image(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte A = r.ReadByte();
                    byte R = r.ReadByte();
                    byte G = r.ReadByte();
                    byte B = r.ReadByte();
                    img[x, y] = new Color32(A, R, G, B);
                }
            }
            return img;
        }

        /// <summary>
        /// deserializes image from existing binary reader
        /// </summary>
        /// <param name="r">the reader we read the obj from</param>
        /// <returns>the image deserialized from the binary reader</returns>
        public static Image Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }

        /// <summary>
        /// creates a black image with given width and height
        /// min size: 1x1
        /// </summary>
        /// <param name="width">the wanted width</param>
        /// <param name="height">the wanted height</param>
        /// <returns>completly black image</returns>
        public static Image Black(int width, int height)
        {
            width = width < 1 ? 1 : width;
            height = height < 1 ? 1 : height;
            Image img = new Image(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    img[x, y] = Color32.Black;
                }
            }
            return img;
        }
        /// <summary>
        /// creates a white image with given width and height
        /// min size: 1x1
        /// </summary>
        /// <param name="width">the wanted width</param>
        /// <param name="height">the wanted height</param>
        /// <returns>completly white image</returns>
        public static Image White(int width, int height)
        {
            width = width < 1 ? 1 : width;
            height = height < 1 ? 1 : height;
            Image img = new Image(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    img[x, y] = Color32.White;
                }
            }
            return img;
        }

        /// <summary>
        /// explicit cast to UnityEngine Texture2D
        /// </summary>
        /// <param name="img">the image that needs casting</param>
        public static explicit operator Texture2D(Image img)
        {
            Texture2D tex = new Texture2D(img.Width, img.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Image.Color32 c = img[x, y];
                    tex.SetPixel(x, y, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
            }
            tex.Apply();
            return tex;
        }
    }

}