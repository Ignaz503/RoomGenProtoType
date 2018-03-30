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
            Default,
            AllSame,
            AlternateAllStyles,
            AlternateOnlyFirstTwoStyles
        }

        public List<TMP_Style> Styles;
        StyleApplicationType Default;

        public Style(StyleApplicationType applyDefault, List<string>styleNames = null)
        {
            Styles = new List<TMP_Style>();
            Default = applyDefault;

            if(styleNames != null)
            {
                foreach(string styleName in styleNames)
                {
                    Styles.Add(TMP_StyleSheet.GetStyle(styleName.Length.GetHashCode()));
                }
            }

        }

        public void ApplyStyle(ref string[] text, StyleApplicationType type)
        {
            int counterIncrease = 1;
            int ceiling = 1;

            StyleApplicationType applyHow = (type == StyleApplicationType.Default) ? Default : type;

            if (Styles.Count > 1)
            {
                switch (applyHow)
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
                    default:
                        counterIncrease = 0;
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

        public void ApplyStyle(ref string text, int styleIdx = 0)
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

    public  class MetaDataAttribute : Attribute
    {
        public string Type;
        public MetaDataAttribute(string type)
        {
            Type = type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override string ToString()
        {
            return Type;
        }
    }
}
