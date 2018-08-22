using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// Represents a Room Style that client gets from server
    /// </summary>
    [Serializable]
    public class RoomStyle : ISerializable
    {
        /// <summary>
        /// Floor texture
        /// </summary>
        [SerializeField]public Image Floor { get; set; }
        /// <summary>
        /// Ceiling Texture
        /// </summary>
        [SerializeField] public Image Ceiling { get; set; }
        /// <summary>
        /// Wall texture
        /// </summary>
        [SerializeField] public Image Wall { get; set; }

        /// <summary>
        /// creates room style
        /// </summary>
        /// <param name="data">
        /// the thre images that make up this style
        /// idx 0: floor,
        /// idx 1: ceiling,
        /// idx 2: wall
        /// </param>
        public RoomStyle(Image[] data)
        {
            if (data.Length != 3)
                throw new Exception($"Not correct data to build roomstyle, need 3 byte arrays, got {data.Length}");

            Floor = data[0];
            Ceiling = data[1];
            Wall = data[2];
        }

        private RoomStyle()
        {

        }

        /// <summary>
        /// Serializes object to byte array
        /// </summary>
        /// <returns>the byte array representing the object</returns>
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
        /// writes object to binary wrtier
        /// </summary>
        /// <param name="w">the writer we write to</param>
        void WriteTo(BinaryWriter w)
        {
            Floor.Serialize(w);
            Ceiling.Serialize(w);
            Wall.Serialize(w);
        }

        /// <summary>
        /// serializes objt into existing binary writer
        /// </summary>
        /// <param name="w">the binary writer we write to</param>
        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }

        /// <summary>
        /// deserializes object from byte array
        /// </summary>
        /// <param name="rS">the obj represented as byte array</param>
        /// <returns>The roomstyle read from the byte array</returns>
        public static RoomStyle Deserialize(byte[] rS)
        {
            using (MemoryStream ms = new MemoryStream(rS))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    return ReadFrom(r);
                }
            }
        }

        /// <summary>
        /// reads obj from a binary reader
        /// </summary>
        /// <param name="r">the reader the obj is read from</param>
        /// <returns>the obj read from the reader</returns>
        static RoomStyle ReadFrom(BinaryReader r)
        {
            RoomStyle rS = new RoomStyle();

            Image f = Image.Deserialize(r);
            Image c = Image.Deserialize(r);
            Image w = Image.Deserialize(r);

            rS.Wall = w;
            rS.Floor = f;
            rS.Ceiling = c;
            return rS;
        }

        /// <summary>
        /// reads obj from existing binray reader
        /// </summary>
        /// <param name="r">the reader we read the obj from</param>
        /// <returns>the obj read from the reader</returns>
        public static RoomStyle Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }

    }

}
