using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class AutoMoveAndRotate : MonoBehaviour
    {
        public Vector3andSpace moveUnitsPerSecond;
        public Vector3andSpace rotateDegreesPerSecond;
        public bool ignoreTimescale;
        private float m_LastRealTime;
        public bool RandomizeStartTimes;
        public bool RandomizeDirection;
        public double MaxTimeBeforeStart;
        [Range(0,1f)]
        public double ChanceForInversion;

        private void Start()
        {
            m_LastRealTime = Time.realtimeSinceStartup;

            System.Random rng = new System.Random((int)DateTime.Now.Ticks);
            if (RandomizeStartTimes)
            {
                moveUnitsPerSecond.TimeBeforeStart = (float)(rng.NextDouble() * MaxTimeBeforeStart);
                rotateDegreesPerSecond.TimeBeforeStart = (float)(rng.NextDouble() * MaxTimeBeforeStart);
            }

            if (RandomizeDirection)
            {
                double decide = rng.NextDouble();
                if(decide <= ChanceForInversion)
                {
                    moveUnitsPerSecond.value = new Vector3(-moveUnitsPerSecond.value.x, -moveUnitsPerSecond.value.y, -moveUnitsPerSecond.value.z);
                    rotateDegreesPerSecond.value = new Vector3(-rotateDegreesPerSecond.value.x, -rotateDegreesPerSecond.value.y, -rotateDegreesPerSecond.value.z);
                }
            }

        }


        // Update is called once per frame
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (ignoreTimescale)
            {
                deltaTime = (Time.realtimeSinceStartup - m_LastRealTime);
                m_LastRealTime = Time.realtimeSinceStartup;
            }
            if (moveUnitsPerSecond.TimeBeforeStart <= 0)
                transform.Translate(moveUnitsPerSecond.value * deltaTime, moveUnitsPerSecond.space);
            else
                moveUnitsPerSecond.TimeBeforeStart -= deltaTime;
            
            if (rotateDegreesPerSecond.TimeBeforeStart <= 0)
                transform.Rotate(rotateDegreesPerSecond.value * deltaTime, moveUnitsPerSecond.space);
            else
                rotateDegreesPerSecond.TimeBeforeStart -= deltaTime;
        }


        [Serializable]
        public class Vector3andSpace
        {
            public Vector3 value;
            public Space space = Space.Self;
            public float TimeBeforeStart = 0f;
        }
    }
}
