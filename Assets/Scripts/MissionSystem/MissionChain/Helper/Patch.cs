using System;
using ParadoxNotion.Design;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    public static class Patch
    {
        /// <summary>get default name of action</summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool FetchAttribute<T>(this ActionBase action, out T attr) where T : Attribute
        {
            var type = action.GetType();
            var attrs = type.GetCustomAttributes(typeof(T), true);
            attr = attrs.Length > 0 ? (T)attrs[0] : null;
            return attr != null;
        }

        /// <summary>change alpha of given color</summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}