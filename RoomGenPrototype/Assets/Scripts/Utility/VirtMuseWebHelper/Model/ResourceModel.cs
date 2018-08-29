using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtMuseWeb.Utility;
using UnityEngine;
using System.Timers;

namespace VirtMuseWeb.Models
{
    public enum ResourceType
    {
        Image,
        Mesh,
        RoomStyle
    }

    #region unused client side
    //public class Resource<T>
    //{
    //    public int ID { get; set; }
    //    public ResourceType Type { get; set; }
    //    public MetaData MetaData { get; set; }
    //    public T[][] Data { get; set; }
    //}
    #endregion

    [DataContract]
    public class ResourceModel
    {
        [DataMember] public int ID { get; set; }
        [DataMember] public ResourceType Type { get; set; }
        [DataMember] public string MetaDataJSON { get; set; }
        [DataMember] public byte[] Data { get; set; }

        /// <summary>
        /// Fills a base display resource with the base data
        /// </summary>
        /// <param name="bR">base display resource</param>
        private void FillBaseDisplayResource(BaseDisplayResource bR)
        {
            bR.ResourecID = ID;
            bR.ResrourceType = Type;
            bR.MetaData = (MetaData)JsonConvert.DeserializeObject<MetaDataModel>(MetaDataJSON);
        }

        public static explicit operator DisplayMeshResource(ResourceModel m)
        {
            if (m.Type != ResourceType.Mesh)
                throw new Exception($"Cannot cast resource of {m.Type} to {typeof(DisplayMeshResource)}");
            DisplayMeshResource mRes = new DisplayMeshResource();

            m.FillBaseDisplayResource(mRes);
            //TODO interaction behaviour setting
            UnityMeshData mD = UnityMeshData.Deserialize(m.Data);
            mRes.Mesh = new Mesh
            {
                vertices = Array.ConvertAll(mD.Vertices, v => v.ToVector3()),
                normals = Array.ConvertAll(mD.Normals, n => n.ToVector3()),
                uv = Array.ConvertAll(mD.UVs, uv => uv.ToVector2()),
                triangles = mD.Triangles
            };
            #region old covnersion
            //Vector3[] vert = new Vector3[mD.Vertices.Length];
            //for (int i = 0; i < mD.Vertices.Length; i++)
            //{
            //    Vec3 v = mD.Vertices[i];
            //    vert[i] = v;
            //}
            //Vector2[] uv = new Vector2[mD.UVs.Length];
            //for (int i = 0; i < mD.UVs.Length; i++)
            //{
            //    Vec2 v = mD.UVs[i];
            //    uv[i] = v;
            //}
            //Vector3[] normals = new Vector3[mD.Normals.Length];
            //for (int i = 0; i < mD.Normals.Length; i++)
            //{
            //    Vec3 v = mD.Normals[i];
            //    normals[i] = v;
            //}
            #endregion

            mRes.Mat = new Material(Shader.Find("Standard"))
            {
                mainTexture = (Texture2D)mD.Texture
            };
            return mRes;
        }

        public static explicit operator DisplayImageResource(ResourceModel m)
        {
            if (m.Type != ResourceType.Image)
                throw new Exception($"Cannot cast resource of {m.Type} to {typeof(DisplayImageResource)}");

            DisplayImageResource imgRes = new DisplayImageResource();
            m.FillBaseDisplayResource(imgRes);
            Image img = Image.Deserialize(m.Data);
            imgRes.Image = (Texture2D)img;
            return imgRes;
        }

        public static explicit operator BaseDisplayResource(ResourceModel m)
        {
            if (m.Type == ResourceType.Image)
                return (DisplayImageResource)m;
            else if (m.Type == ResourceType.Mesh)
                return (DisplayMeshResource)m;
            else
                throw new Exception($"Cannot convert {m.Type} to BaseDisplayResource");
        }

        public static explicit operator RoomStyleResource(ResourceModel m)
        {
            if (m.Type != ResourceType.RoomStyle)
                throw new Exception($"Cannot cast resource of type{m.Type} to RoomStyleResource");

            Utility.RoomStyle style = Utility.RoomStyle.Deserialize(m.Data);

            return new RoomStyleResource()
            {
                ResourceID = m.ID,
                Floor = (Texture2D)style.Floor,
                Ceiling = (Texture2D)style.Ceiling,
                Wall = (Texture2D)style.Wall
            };
                       
        }

        /// <summary>
        /// Converison to appliable mesh resource as coroutine
        /// </summary>
        /// <param name="mRes">the meshResource that should be fill</param>
        public IEnumerator ToDisplayMeshResource(DisplayMeshResource mRes)
        {
            if (Type != ResourceType.Mesh)
                throw new Exception($"Cannot cast resource of {Type} to {typeof(DisplayMeshResource)}");

            FillBaseDisplayResource(mRes);
            mRes.Mesh = new Mesh();
            //TODO interaction behaviour setting
            UnityMeshData mD = UnityMeshData.Deserialize(Data);

            #region old covnersion

            Vector3[] vert = new Vector3[mD.Vertices.Length];
            for (int i = 0; i < mD.Vertices.Length; i++)
            {
                Vec3 v = mD.Vertices[i];
                vert[i] = v;
            }

            yield return null;

            Vector2[] uv = new Vector2[mD.UVs.Length];
            for (int i = 0; i < mD.UVs.Length; i++)
            {
                Vec2 v = mD.UVs[i];
                uv[i] = v;
            }

            yield return null;
            Vector3[] normals = new Vector3[mD.Normals.Length];
            for (int i = 0; i < mD.Normals.Length; i++)
            {
                Vec3 v = mD.Normals[i];
                normals[i] = v;
            }
            mRes.Mesh.vertices = vert;
            mRes.Mesh.normals = normals;
            mRes.Mesh.uv = uv;
            mRes.Mesh.triangles = mD.Triangles;
            yield return null;
            #endregion

            mRes.Mat = new Material(Shader.Find("Standard"));
            Texture2D tex = new Texture2D(mD.Texture.Width, mD.Texture.Height);

            for (int x = 0; x < mD.Texture.Width; x++)
            {
                for (int y = 0; y < mD.Texture.Height; y++)
                {
                    Image.Color32 c = mD.Texture[x, y];
                    tex.SetPixel(x, y, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
            }
            yield return null;
            tex.Apply();
            mRes.Mat.mainTexture = tex;
        }

        /// <summary>
        /// conversion to appliable image resource as coroutine
        /// </summary>
        /// <param name="imgRes">the image resource that should be filled</param>
        public IEnumerator ToDisplayImageResource(DisplayImageResource imgRes)
        {
            if (Type != ResourceType.Image)
                throw new Exception($"Cannot cast resource of {Type} to {typeof(DisplayImageResource)}");

            FillBaseDisplayResource(imgRes);
            Image img = Image.Deserialize(Data);
            yield return null;
            Texture2D tex = new Texture2D(img.Width, img.Height);
            for (int x = 0; x < img.Width; x++)
            {
               
                for (int y = 0; y < img.Height; y++)
                {
                    Image.Color32 c =img[x, y];
                    tex.SetPixel(x, y, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
            }
            yield return null;
            tex.Apply();
            imgRes.Image = tex;
        }

        public IEnumerator ToRoomStyleResource(RoomStyleResource style)
        {
            if (Type != ResourceType.RoomStyle)
                throw new Exception($"Cannot cast resource of type{Type} to RoomStyleResource");

            Utility.RoomStyle st = Utility.RoomStyle.Deserialize(Data);
            style.ResourceID = ID;
            yield return null;
            Texture2D floor = new Texture2D(st.Floor.Height,st.Floor.Width);
            for (int x = 0; x < st.Floor.Width; x++)
            {
                
                for (int y = 0; y < st.Floor.Height; y++)
                {
                    Image.Color32 c = st.Floor[x, y];
                    floor.SetPixel((floor.width - 1) - y, (floor.width - 1) - x, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
                
            }
            yield return null;
            floor.Apply();
            style.Floor = floor;
            yield return null;
            Texture2D ceiling = new Texture2D(st.Ceiling.Height, st.Ceiling.Width);
            for (int x = 0; x < st.Ceiling.Width; x++)
            {
               
                for (int y = 0; y < st.Ceiling.Height; y++)
                {
                    Image.Color32 c = st.Ceiling[x, y];
                    ceiling.SetPixel((ceiling.width - 1) - y, (ceiling.width - 1) - x, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
                
            }
            yield return null;
            ceiling.Apply();
            style.Ceiling = ceiling;
            yield return null;
            Texture2D wall = new Texture2D(st.Wall.Height, st.Wall.Width);
            for (int x = 0; x < st.Wall.Width; x++)
            {
                
                for (int y = 0; y < st.Wall.Height; y++)
                {
                    Image.Color32 c = st.Wall[x, y];
                    wall.SetPixel((wall.width - 1) - y, (wall.width - 1) - x, new UnityEngine.Color32(c.R, c.G, c.B, c.A));
                }
            }
            yield return null;
            wall.Apply();
            style.Wall = wall;
        }
    }
}
