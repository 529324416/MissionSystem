#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;

namespace RedSaw.MissionSystem
{
    public static class EditorHelper
    {
        
        /// <summary>change alpha of given color</summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }


        /// <summary>绘制标题栏</summary>
        /// <param name="isUnfolded"></param>
        /// <param name="label"></param>
        /// <param name="openScript"></param>
        /// <param name="menu"></param>
        /// <returns></returns>
        public static bool DrawTitleBar(bool isUnfolded, string label, Action openScript, GenericMenu menu)
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label(
                    "<b>" + (isUnfolded ? "▼ " : "► ") + label + "</b>"
                    , Styles.leftLabel);

                if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
                    openScript?.Invoke();

                GUI.color = Color.grey;
                if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20),
                        GUILayout.Height(20)))
                    menu.ShowAsContext();
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
            
            var titleRect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);
            GUI.color = Color.black.WithAlpha(0.25f);
            GUI.DrawTexture(new Rect(titleRect.x, titleRect.yMax - 1, titleRect.width, 1), Texture2D.whiteTexture);
            GUI.color = Color.white;
            var e = Event.current;
            if ( e.type == EventType.ContextClick && titleRect.Contains(e.mousePosition) )
            {
                menu.ShowAsContext();
                e.Use();
            }
            
            if ( e.button == 0 && e.type == EventType.MouseUp && titleRect.Contains(e.mousePosition) ) 
            {
                isUnfolded = !isUnfolded;
                e.Use();
            }

            return isUnfolded;
        }
    }
}
#endif
