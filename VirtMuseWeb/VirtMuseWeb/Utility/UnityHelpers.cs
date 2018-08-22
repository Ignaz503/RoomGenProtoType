using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UnityEngine;

/* FastObjImporter.cs
* by Marc Kusters (Nighteyes) 
* slightly modified by Lukas Neuhold
* Modifications: made mutlithreadable, by not making it a singelton
*                moved from unity Vec3 and so on to own vec3 implementation
* http://wiki.unity3d.com/index.php/FastObjImporter
* 
* Used for loading .obj files exported by Blender
* Example usage: Mesh myMesh = FastObjImporter.Instance.ImportFile("path_to_obj_file.obj");
* Content is available under Creative Commons Attribution Share Alike.
* https://creativecommons.org/licenses/by-sa/3.0/
*/
namespace VirtMuseWeb.Utility
{
    public interface ISerializable
    {
        byte[] Serialize();
        void Serialize(BinaryWriter w);
    }

    [Serializable]
    public class UnityMeshData : ISerializable
    {

        [SerializeField] public int[] Triangles { get; set; }
        [SerializeField] public Vec3[] Vertices { get; set; }
        [SerializeField] public Vec2[] UVs { get; set; }
        [SerializeField] public Vec3[] Normals { get; set; }
        [SerializeField] public Utility.Image Texture { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// serializes to byte array
        /// </summary>
        /// <returns></returns>
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

        void WriteTo(BinaryWriter w)
        {
            w.Write((Int32)Triangles.Length);
            foreach (int t in Triangles)
            {
                w.Write(t);
            }
            w.Write((Int32)Vertices.Length);
            foreach (Vec3 v in Vertices)
            {
                w.Write(v.X);
                w.Write(v.Y);
                w.Write(v.Z);
            }
            w.Write(UVs.Length);
            for (int i = 0; i < UVs.Length; i++)
            {
                Vec2 v = UVs[i];
                if(v == null)
                {
                    w.Write(0.0f);
                    w.Write(0.0f);
                }
                else
                {
                    w.Write(v.X);
                    w.Write(v.Y);
                }
            }
            w.Write((Int32)Normals.Length);
            foreach (Vec3 v in Normals)
            {
                w.Write(v.X);
                w.Write(v.Y);
                w.Write(v.Z);
            }
            Texture.Serialize(w);
        }

        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }
        
        /// <summary>
        /// deserializes unity mesh data objects
        /// doesn't care if correctly or not
        /// </summary>
        /// <param name="obj">the obj as a bayte array</param>
        /// <returns>the desserialized object</returns>
        public static UnityMeshData Deserialize(byte[] obj)
        {
            using (MemoryStream ms = new MemoryStream(obj))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    return ReadFrom(r);
                }
            }
        }

        public static UnityMeshData Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }

        static UnityMeshData ReadFrom(BinaryReader r)
        {
            int triLength = r.ReadInt32();
            int[] triangles = new int[triLength];
            for (int i = 0; i < triLength; i++)
            {
                int triIdx = r.ReadInt32();
                triangles[i] = triIdx;
            }

            int vertLength = r.ReadInt32();
            Vec3[] vertices = new Vec3[vertLength];
            for(int i = 0; i < vertLength; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                float z = r.ReadSingle();
                vertices[i] = new Vec3(x, y, z);
            }

            int uvLength = r.ReadInt32();
            Vec2[] uvs = new Vec2[uvLength];
            for (int i = 0; i < uvLength; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                uvs[i] = new Vec2(x, y);
            }

            int normLength = r.ReadInt32();
            Vec3[] normals = new Vec3[normLength];
            for (int i = 0; i < normLength; i++)
            {
                float x = r.ReadSingle();
                float y = r.ReadSingle();
                float z = r.ReadSingle();

                normals[i] = new Vec3(x, y, z);
            }
            Image tex = Image.Deserialize(r);

            return new UnityMeshData() { Triangles = triangles, Vertices = vertices, UVs = uvs, Normals = normals, Texture = tex };
        }
    }

    [Serializable]
    public class Image : ISerializable
    {
        [Serializable]
        public struct Color32
        {
            [SerializeField] public byte R { get; set; }
            [SerializeField] public byte G { get; set; }
            [SerializeField] public byte B { get; set; }
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

        [SerializeField]
        public int Width { get; set; }
        [SerializeField]
        public int Height { get; set; }
        [SerializeField]
        Color32[,] color;

        public Color32 this[int x, int y] { get { return color[x,y]; } set { color[x,y] = value; } }

        public Image(int w, int h)
        {
            Width = w;
            Height = h;
            color = new Color32[Width,Height];
        }

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

        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }

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

        public static Image Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }
       
        public static Image Black(int width, int height)
        {
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
        public static Image White(int width, int height)
        {
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
    }

    public sealed class FastObjImporter
    {
        //TODO not file but string already
        private List<int> triangles;
        private List<Vec3> vertices;
        private List<Vec2> uv;
        private List<Vec3> normals;
        private List<Vec3Int> faceData;
        private List<int> intArray;

        private const int MIN_POW_10 = -16;
        private const int MAX_POW_10 = 16;
        private const int NUM_POWS_10 = MAX_POW_10 - MIN_POW_10 + 1;
        private static readonly float[] pow10 = GenerateLookupTable();

        // Use this for initialization
        public UnityMeshData BuildMesh(string OBJString)
        {
            triangles = new List<int>();
            vertices = new List<Vec3>();
            uv = new List<Vec2>();
            normals = new List<Vec3>();
            faceData = new List<Vec3Int>();
            intArray = new List<int>();

            LoadMeshData(OBJString);

            Vec3[] newVerts = new Vec3[faceData.Count];
            Vec2[] newUVs = new Vec2[faceData.Count];
            Vec3[] newNormals = new Vec3[faceData.Count];

            /* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
             * for the appropriate Unity mesh array.
             */
            for (int i = 0; i < faceData.Count; i++)
            {
                newVerts[i] = vertices[faceData[i].x - 1];
                if (faceData[i].y >= 1)
                    newUVs[i] = uv[faceData[i].y - 1];

                if (faceData[i].z >= 1)
                    newNormals[i] = normals[faceData[i].z - 1];
            }

            UnityMeshData mesh = new UnityMeshData
            {
                Vertices = newVerts,
                UVs = newUVs,
                Normals = newNormals,
                Triangles = triangles.ToArray()
            };

            return mesh;
        }

        private void LoadMeshData(string OBJString)
        {
            //ensure \n not  \r\n
            OBJString.Replace("\r\n", "\n");

            StringBuilder sb = new StringBuilder();
            int start = 0;
            string objectName = null;
            int faceDataCount = 0;

            StringBuilder sbFloat = new StringBuilder();

            for (int i = 0; i < OBJString.Length; i++)
            {
                if (OBJString[i] == '\n')
                {
                    sb.Remove(0, sb.Length);

                    // Start +1 for whitespace '\n'
                    sb.Append(OBJString, start + 1, i - start);
                    start = i;

                    if (sb[0] == 'o' && sb[1] == ' ')
                    {
                        sbFloat.Remove(0, sbFloat.Length);
                        int j = 2;
                        while (j < sb.Length)
                        {
                            objectName += sb[j];
                            j++;
                        }
                    }
                    else if (sb[0] == 'v' && sb[1] == ' ') // Vertices
                    {
                        int splitStart = 2;

                        vertices.Add(new Vec3(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 't' && sb[2] == ' ') // UV
                    {
                        int splitStart = 3;

                        uv.Add(new Vec2(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 'n' && sb[2] == ' ') // Normals
                    {
                        int splitStart = 3;

                        normals.Add(new Vec3(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'f' && sb[1] == ' ')
                    {
                        int splitStart = 2;

                        int j = 1;
                        intArray.Clear();
                        int info = 0;
                        // Add faceData, a face can contain multiple triangles, facedata is stored in following order vert, uv, normal. If uv or normal are / set it to a 0
                        while (splitStart < sb.Length && char.IsDigit(sb[splitStart]))
                        {
                            faceData.Add(new Vec3Int(GetInt(sb, ref splitStart, ref sbFloat),
                                GetInt(sb, ref splitStart, ref sbFloat), GetInt(sb, ref splitStart, ref sbFloat)));
                            j++;

                            intArray.Add(faceDataCount);
                            faceDataCount++;
                        }

                        info += j;
                        j = 1;
                        while (j + 2 < info) //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                        {
                            triangles.Add(intArray[0]);
                            triangles.Add(intArray[j]);
                            triangles.Add(intArray[j + 1]);

                            j++;
                        }
                    }
                }
            }
        }

        private float GetFloat(StringBuilder sb, ref int start, ref StringBuilder sbFloat)
        {
            sbFloat.Remove(0, sbFloat.Length);
            while (start < sb.Length &&
                   (char.IsDigit(sb[start]) || sb[start] == '-' || sb[start] == '.'))
            {
                sbFloat.Append(sb[start]);
                start++;
            }
            start++;

            return ParseFloat(sbFloat);
        }

        private int GetInt(StringBuilder sb, ref int start, ref StringBuilder sbInt)
        {
            sbInt.Remove(0, sbInt.Length);
            while (start < sb.Length &&
                   (char.IsDigit(sb[start])))
            {
                sbInt.Append(sb[start]);
                start++;
            }
            start++;

            return IntParseFast(sbInt);
        }


        private static float[] GenerateLookupTable()
        {
            var result = new float[(-MIN_POW_10 + MAX_POW_10) * 10];
            for (int i = 0; i < result.Length; i++)
                result[i] = (float)((i / NUM_POWS_10) *
                        MathF.Pow(10, i % NUM_POWS_10 + MIN_POW_10));
            return result;
        }

        private float ParseFloat(StringBuilder value)
        {
            float result = 0;
            bool negate = false;
            int len = value.Length;
            int decimalIndex = value.Length;
            for (int i = len - 1; i >= 0; i--)
                if (value[i] == '.')
                { decimalIndex = i; break; }
            int offset = -MIN_POW_10 + decimalIndex;
            for (int i = 0; i < decimalIndex; i++)
                if (i != decimalIndex && value[i] != '-')
                    result += pow10[(value[i] - '0') * NUM_POWS_10 + offset - i - 1];
                else if (value[i] == '-')
                    negate = true;
            for (int i = decimalIndex + 1; i < len; i++)
                if (i != decimalIndex)
                    result += pow10[(value[i] - '0') * NUM_POWS_10 + offset - i];
            if (negate)
                result = -result;
            return result;
        }

        private int IntParseFast(StringBuilder value)
        {
            // An optimized int parse method.
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                result = 10 * result + (value[i] - 48);
            }
            return result;
        }
    }

    [Serializable]
    public class RoomStyle :ISerializable
    {
        [SerializeField]public Image Floor { get; set; }
        [SerializeField] public Image Ceiling { get; set; }
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
        public RoomStyle(byte[][] data)
        {
            if (data.Length != 3)
                throw new Exception($"Not correct data to build roomstyle, need 3 byte arrays, got {data.Length}");

            Floor = ImageHelper.GetImage(data[0]);
            Ceiling = ImageHelper.GetImage(data[1]);
            Wall = ImageHelper.GetImage(data[2]);
        }

        private RoomStyle()
        {

        }

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

        void WriteTo(BinaryWriter w)
        {
            Floor.Serialize(w);
            Ceiling.Serialize(w);
            Wall.Serialize(w);
        }

        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }

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

        public static RoomStyle Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }
    }

}
