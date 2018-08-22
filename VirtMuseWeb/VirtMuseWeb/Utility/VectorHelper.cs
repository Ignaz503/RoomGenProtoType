using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    [Serializable]
    public class Vec3
    {
        [SerializeField] public float X { get; set; }
        [SerializeField] public float Y { get; set; }
        [SerializeField] public float Z { get; set; }

        public Vec3():this(0,0,0)
        { }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vec3 zero { get { return new Vec3(0, 0, 0); } }
        public static Vec3 one { get { return new Vec3(1, 1, 1); } }
        public static Vec3 right { get { return new Vec3(1, 0, 0); } }
        public static Vec3 left { get { return new Vec3(-1, 0, 0); } }
        public static Vec3 down { get { return new Vec3(0, -1, 0); } }
        public static Vec3 up { get { return new Vec3(0, 1, 0); } }
        public static Vec3 forward { get { return new Vec3(0, 0, 1); } }
        public static Vec3 back { get { return new Vec3(0, 0, -1); } }
    }

    [Serializable]
    public class Vec2
    {
        [SerializeField] public float X { get; set; }
        [SerializeField] public float Y { get; set; }

        public Vec2() :this(0,0)
        {}

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 zero { get { return new Vec2(0, 0); } }
        public static Vec2 one { get { return new Vec2(1, 1); } }
        public static Vec2 right { get { return new Vec2(1, 0); } }
        public static Vec2 left { get { return new Vec2(-1, 0); } }
        public static Vec2 down { get { return new Vec2(0, -1); } }
        public static Vec2 up { get { return new Vec2(0, 1); } }
    }

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
