using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// Vector 2 helper class that is seriaizable
    /// usefull when trying to use binary formatters for serializing
    /// used by the server for serialization
    /// </summary>
    [Serializable]
    public class Vec2
    {
        [SerializeField] public float X { get; set; }
        [SerializeField] public float Y { get; set; }

        public Vec2() : this(0, 0)
        { }

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2(Vector2 v) : this(v.x, v.y)
        { }

        /// <summary>
        /// Returns a UnityEngine Vector2 with coords this vector
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2() { return new Vector2(X, Y); }

        /// <summary>
        /// implicit cast to UnityEngine Vector2
        /// </summary>
        /// <param name="v">the Vec2 that needs casting</param>
        public static implicit operator Vector2(Vec2 v)
        {
            return v.ToVector2();
        }

        public static Vec2 zero { get { return new Vec2(0, 0); } }
        public static Vec2 one { get { return new Vec2(1, 1); } }
        public static Vec2 right { get { return new Vec2(1, 0); } }
        public static Vec2 left { get { return new Vec2(-1, 0); } }
        public static Vec2 down { get { return new Vec2(0, -1); } }
        public static Vec2 up { get { return new Vec2(0, 1); } }
    }
}