using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// serializable vector 3 wih integer coords
    /// useful when using binary formatters
    /// created by the creator of the fast object importer
    /// </summary>
    [Serializable]
    public class Vec3Int
    {
        [SerializeField] public int x { get; set; }
        [SerializeField] public int y { get; set; }
        [SerializeField] public int z { get; set; }

        public Vec3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3Int()
        : this(0,0,0)
        {}

        public Vec3Int(Vector3Int v): this(v.x,v.y,v.z)
        {}

        /// <summary>
        /// Returns UnityEngine Vector3Int
        /// </summary>
        /// <returns></returns>
        public Vector3Int ToVector3Int() { return new Vector3Int(x, y, z); }

        /// <summary>
        /// implicit cast to UnityEngine Vector3Int
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Vector3Int(Vec3Int v)
        {
            return v.ToVector3Int();
        }

        public static Vec3Int zero { get { return new Vec3Int(0, 0, 0); } }
        public static Vec3Int one { get { return new Vec3Int(1, 1, 1); } }
        public static Vec3Int right { get { return new Vec3Int(1, 0, 0); } }
        public static Vec3Int left { get { return new Vec3Int(-1, 0, 0); } }
        public static Vec3Int down { get { return new Vec3Int(0, -1, 0); } }
        public static Vec3Int up { get { return new Vec3Int(0, 1, 0); } }
        public static Vec3Int forward { get { return new Vec3Int(0, 0, 1); } }
        public static Vec3Int back { get { return new Vec3Int(0, 0, -1); } }

    }
}
