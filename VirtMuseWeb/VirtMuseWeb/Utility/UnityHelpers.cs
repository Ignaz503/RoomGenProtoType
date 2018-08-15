using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
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
    [Serializable]
    public class UnityMeshData
    {

        public int[] Triangles { get; set; }
        public Vector3[] Vertices { get; set; }
        public Vector2[] UVs { get; set; }
        public Vector3[] Normals { get; set; }
        public byte[] Texture { get; set; }

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
                BinaryFormatter bF = new BinaryFormatter();
                bF.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// deserializes unity mesh data objects
        /// doesn't care if correctly or not
        /// </summary>
        /// <param name="obj">the obj as a bayte array</param>
        /// <returns>the desserialized object</returns>
        public static UnityMeshData Deserialize(byte[] obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(obj, 0, obj.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(ms) as UnityMeshData;
            }
        }
    }

    public sealed class FastObjImporter
    {
        //TODO not file but string already
        private List<int> triangles;
        private List<Vector3> vertices;
        private List<Vector2> uv;
        private List<Vector3> normals;
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
            vertices = new List<Vector3>();
            uv = new List<Vector2>();
            normals = new List<Vector3>();
            faceData = new List<Vec3Int>();
            intArray = new List<int>();

            LoadMeshData(OBJString);

            Vector3[] newVerts = new Vector3[faceData.Count];
            Vector2[] newUVs = new Vector2[faceData.Count];
            Vector3[] newNormals = new Vector3[faceData.Count];

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

                        vertices.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat), GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 't' && sb[2] == ' ') // UV
                    {
                        int splitStart = 3;

                        uv.Add(new Vector2(GetFloat(sb, ref splitStart, ref sbFloat),
                            GetFloat(sb, ref splitStart, ref sbFloat)));
                    }
                    else if (sb[0] == 'v' && sb[1] == 'n' && sb[2] == ' ') // Normals
                    {
                        int splitStart = 3;

                        normals.Add(new Vector3(GetFloat(sb, ref splitStart, ref sbFloat),
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
    public class RoomStyle
    {
        public byte[] Floor { get; set; }
        public byte[] Ceiling { get; set; }
        public byte[] Wall { get; set; }
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

            Floor = data[0];
            Ceiling = data[1];
            Wall = data[2];
        }

        public byte[] Serialize()
        {
            BinaryFormatter bF = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bF.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static RoomStyle Deserialize(byte[] rS)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(rS, 0, rS.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return bf.Deserialize(ms) as RoomStyle;
            }
        }

    }

}
