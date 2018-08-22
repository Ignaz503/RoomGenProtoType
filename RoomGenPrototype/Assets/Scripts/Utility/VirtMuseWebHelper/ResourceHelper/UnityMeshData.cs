using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// Helper Class that holds information to build a unity mesh and
    /// is serializable, used by server to store mesh data
    /// </summary>
    [Serializable]
    public class UnityMeshData : ISerializable
    {
        /// <summary>
        /// triangles in mesh
        /// </summary>
        [SerializeField] public int[] Triangles { get; set; }
        /// <summary>
        /// vertices in mesh
        /// </summary>
        [SerializeField] public Vec3[] Vertices { get; set; }
        /// <summary>
        /// uv coords of vertices in mesh
        /// </summary>
        [SerializeField] public Vec2[] UVs { get; set; }
        /// <summary>
        /// normals of mesh
        /// </summary>
        [SerializeField] public Vec3[] Normals { get; set; }
        /// <summary>
        /// texture of mesh
        /// </summary>
        [SerializeField] public Utility.Image Texture { get; set; }

        /// <summary>
        /// To string that returns object in json format
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Wrtites object to binary writer
        /// </summary>
        /// <param name="w">the binary writer it needs to be written to</param>
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
                if (v == null)
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

        /// <summary>
        /// serializes object into already existing binary writer
        /// </summary>
        /// <param name="w">the binary writer we need to write to</param>
        public void Serialize(BinaryWriter w)
        {
            WriteTo(w);
        }
        
        /// <summary>
        /// Deserializes unity mesh data from a byte array
        /// </summary>
        /// <param name="obj">the obj in byte array representation</param>
        /// <returns>the UnityMeshData read from the byte array</returns>
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

        /// <summary>
        /// Deserializes UnityMesh data from an existing binray reader
        /// </summary>
        /// <param name="r">the binary reader we read the mesh data from</param>
        /// <returns>the UnityMeshData we read from the reader</returns>
        public static UnityMeshData Deserialize(BinaryReader r)
        {
            return ReadFrom(r);
        }

        /// <summary>
        /// Reads UnityMeshData from binary reader
        /// </summary>
        /// <param name="r">The binary reader</param>
        /// <returns>the deserialized UnityMeshData</returns>
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
            for (int i = 0; i < vertLength; i++)
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

}