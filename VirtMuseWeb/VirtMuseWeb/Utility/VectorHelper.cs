using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Utility
{
    public class Vec3Int
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public Vec3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3Int()
        : this(0,0,0)
        {}

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
