#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;

namespace RedSaw.MissionSystem
{
    public static class EditorHelper
    {
        /// <summary>add new menu item to current menu</summary>
        /// <param name="menu"></param>
        /// <param name="title"></param>
        /// <param name="isDisabled"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static GenericMenu AddMenuItem(this GenericMenu menu, string title, bool isDisabled, Action callback = null)
        {
            if (isDisabled)
            {
                menu.AddDisabledItem(new GUIContent(title));
            }
            else
            {
                menu.AddItem(new GUIContent(title), false, () => callback?.Invoke());
            }
            return menu;
        }
        
        /// <summary>add menu separator</summary>
        /// <param name="menu"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GenericMenu AddMenuSeparator(this GenericMenu menu, string path = "/")
        {
            menu.AddSeparator(path);
            return menu;
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


        // public static bool ShowTitlebar<T>(T obj, Action<T> callback, bool isUnfolded, Action deleteCallback = null) 
        // {
        //     GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.black.WithAlpha(0.3f) : Color.white.WithAlpha(0.5f);
        //     GUILayout.BeginHorizontal(GUI.skin.box);
        //     {
        //         bool isValid = RequireTemplate.CheckValid(out string invalidReason);
        //         GUI.backgroundColor = Color.white;
        //         GUILayout.Label(
        //             "<b>" + (isUnfolded ? "▼ " : "► ") + RequireTemplate.SummaryInfo + "</b>" +
        //             // (isUnfolded ? "" : "\n<i><size=12>(" + require.Description + ")</size></i>") +
        //             ((hasAdditionalCondition && additionalCondition != null) ? $"\n<i><size=12>附加条件:{additionalCondition.taskTitle}</size></i>" : "" + 
        //             (isValid ? "" : $"\n<i><color=red>{invalidReason}</color></i>"))
        //                 , Styles.leftLabel);
        //
        //         if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
        //             EditorUtils.OpenScriptOfType(RequireTemplate.GetType());
        //
        //         GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.grey;
        //         if ( GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20), GUILayout.Height(20)) )
        //         {
        //             GetMenu(callback, deleteCallback).ShowAsContext();
        //         }
        //         GUI.color = Color.white;
        //     }
        //     GUILayout.EndHorizontal();
        //
        //     var titleRect = GUILayoutUtility.GetLastRect();
        //     EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);
        //     GUI.color = Color.black.WithAlpha(0.25f);
        //     GUI.DrawTexture(new Rect(titleRect.x, titleRect.yMax - 1, titleRect.width, 1), Texture2D.whiteTexture);
        //     GUI.color = Color.white;
        //
        //     var e = Event.current;
        //     // if ( e.type == EventType.ContextClick && titleRect.Contains(e.mousePosition) ) 
        //     // {
        //     //     GetMenu(callback, deleteCallback).ShowAsContext();
        //     //     e.Use();
        //     // }
        //
        //     if ( e.button == 0 && e.type == EventType.MouseUp && titleRect.Contains(e.mousePosition) ) 
        //     {
        //         isUnfolded = !isUnfolded;
        //         e.Use();
        //     }
        //
        //     return isUnfolded;
        // }
    }
}
#endif
