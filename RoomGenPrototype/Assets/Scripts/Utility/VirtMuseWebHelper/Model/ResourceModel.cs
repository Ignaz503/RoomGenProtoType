using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtMuseWeb.Utility;
using UnityEngine;

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
    }
}
