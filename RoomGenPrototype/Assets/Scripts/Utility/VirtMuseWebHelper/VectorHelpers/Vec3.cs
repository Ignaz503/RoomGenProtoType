using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// Serialiazable Vector3 that is used by the server to build 
    /// all the UnityHelper things
    /// usefull when wanting to use binaryformatters for serializing things
    /// </summary>
    [Serializable]
    public class Vec3
    {
        [SerializeField] public float X { get; set; }
        [SerializeField] public float Y { get; set; }
        [SerializeField] public float Z { get; set; }

        public Vec3() : this(0, 0, 0)
        { }

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(Vector3 v) : this(v.x, v.y, v.z)
        { }

        /// <summary>
        /// returns a UnityEngine Vector3
        /// </summary>
        /// <returns></returns>
        public Vector3 ToVector3() { return new Vector3(X, Y, Z); }

        /// <summary>
        /// implicit cast to UnityEngine Vector3
        /// </summary>
        /// <param name="v">The vector that needs casting</param>
        public static implicit operator Vector3(Vec3 v)
        {
            return v.ToVector3();
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
}
