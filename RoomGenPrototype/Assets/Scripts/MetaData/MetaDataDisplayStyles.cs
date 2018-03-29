using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MetaDataDisplayStyles
{
    public class Style
    {
        public enum StyleApplicationType
        {
            AllSame,
            AlternateAllStyles,
            AlternateOnlyFirstTwoStyles
        }

        public List<TMP_Style> Styles;

        public Style()
        {
            Styles = new List<TMP_Style>();
        }

        void ApplyStyle(ref string[] text, StyleApplicationType type)
        {
            int counterIncrease = 1;
            int ceiling = 1;

            if (Styles.Count > 1)
            {
                switch (type)
                {
                    case StyleApplicationType.AllSame:
                        counterIncrease = 0;
                        break;
                    case StyleApplicationType.AlternateAllStyles:
                        ceiling = Styles.Count;
                        break;
                    case StyleApplicationType.AlternateOnlyFirstTwoStyles:
                        ceiling = 2;
                        break;
                }
            }
            else
            {
                counterIncrease = 0;
            }
            int styleIdx = 0;
            for (int i = 0; i < text.Length; i++)
            {
                ApplyStyle(ref text[i], styleIdx);
                styleIdx = (styleIdx + counterIncrease) % ceiling;
            }


        }

        void ApplyStyle(ref string text, int styleIdx = 0)
        {
            if (styleIdx < 0)
                styleIdx = 0;
            if (styleIdx >= Styles.Count)
                styleIdx = Styles.Count - 1;

            text.Insert(0, Styles[styleIdx].styleOpeningDefinition);
            text += Styles[styleIdx].styleClosingDefinition;
        }

    }
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]

    public  class MetaDataAttributeAttribute : Attribute
    {
        public string Type;
        public MetaDataAttributeAttribute(string type)
        {
            Type = type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
    
    public class MetaDataStylesController : MonoBehaviour
    {
        public static MetaDataStylesController Instance;

        public Dictionary<MetaDataAttributeAttribute, Style> MetaDataTypeToStyleMapping;

        private void Awake()
        {
            if(Instance != null)
            {
                throw new Exception("There already exists a metadata style controller");
            }
            Instance = this;   
        }

    }
}
