#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using PopupWindow = UnityEditor.PopupWindow;

namespace RedSaw.Editor
{
    /// <summary>
    /// a simple universal dropdown menu implementation based on Unity PopupWindowContent
    /// <para>
    /// example:
    /// <code>
    ///     DropdownMenu.MakeMenu("label", value, options, r => value = r, "error message");
    /// </code> 
    /// </para>
    /// </summary>
    public class DropdownMenu : PopupWindowContent
    {
        private readonly static GUIStyle fieldButtonStyle =
            new GUIStyle(EditorStyles.popup) { alignment = TextAnchor.MiddleLeft, };

        public static void MakeMenu(
            string label,
            string currentOption,
            string[] options,
            Action<string> callback,
            string errorMessage = "Has No Items!!"
        )
        {
            currentOption ??= string.Empty;
            var style = fieldButtonStyle;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(currentOption), style, GUILayout.ExpandWidth(true));
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (GUI.Button(rect, currentOption, style))
            {
                var menuInstance = new DropdownMenu(callback, options, errorMessage);
                PopupWindow.Show(rect, menuInstance);
            }

            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.EndHorizontal();
        }

        void Callback(string result)
        {
            _callback?.Invoke(result);
            editorWindow.Close();
        }

        private string[] options;
        public string[] Options { get => options; set => options = value ?? Array.Empty<string>(); }

        private const int selectionHeight = 23;
        private readonly Color hoverColor = new Color(0.5f, 0.5f, 1, 0.3f);
        
        private readonly string errorMsg;
        private string[] filteredOptions;
        private string filterString = string.Empty;
        private Vector2 scrollPosition = Vector2.zero;
        private int selectedIndex = -1;
        private int lastSelectedIndex = -1;
        private bool shouldRepaint;
        private readonly Action<string> _callback;
        

        private DropdownMenu(Action<string> callback, string[] options, string errorMsg = "has no items!!") : base()
        {
            this._callback = callback;
            this.options = options;
            this.errorMsg = errorMsg;

            filteredOptions = new string[options.Length];
            Array.Copy(options, filteredOptions, options.Length);
        }
        
        public override Vector2 GetWindowSize()
        {
            Vector2 size = base.GetWindowSize();
            size.x += 80;
            size.y = Mathf.Min(320, Mathf.Max((options.Length + 1) * selectionHeight + 20, 200));
            return size;
        }

        public override void OnOpen()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        public override void OnClose()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        void OnEditorUpdate()
        {
            if (!shouldRepaint) return;
            editorWindow.Repaint();
            shouldRepaint = false;
        }

        public override void OnGUI(Rect rect)
        {
            ParadoxNotion.Design.Styles.Draw(rect, ParadoxNotion.Design.Styles.shadowedBackground);
            if (options == null || options.Length == 0)
            {
                GUILayout.Label(errorMsg);
                return;
            }
            
            var e = Event.current;
            HandleEvent(e);
            DrawDropdownMenu(e);
        }

        private void HandleEvent(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.DownArrow)
                {
                    selectedIndex = Mathf.Min(selectedIndex + 1, filteredOptions.Length - 1);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Return)
                {
                    this.Callback(filteredOptions[Mathf.Max(selectedIndex, 0)]);
                    editorWindow.Close();
                }
            }
        }

        private void DrawDropdownMenu(Event e)
        {
            lastSelectedIndex = selectedIndex;
            EditorGUI.BeginChangeCheck();
            filterString = EditorGUILayout.TextField(filterString, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
            {
                selectedIndex = -1;
                if (string.IsNullOrEmpty(filterString))
                {
                    filteredOptions = new string[options.Length];
                    Array.Copy(options, filteredOptions, options.Length);
                }
                else
                {
                    filteredOptions = options.Where(option => option.Contains(filterString)).ToArray();
                }
            }
            

            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
            
            for (int i = 0; i < filteredOptions.Length; i++)
            {
                GUI.enabled = true;
                {
                    GUI.color = Color.white;
                    var label = $"<size=11><color=#a8a8a8>●  {filteredOptions[i]}</color></size>";
                    GUILayout.Label(label, ParadoxNotion.Design.Styles.leftLabel, GUILayout.Width(0),
                        GUILayout.Height(selectionHeight),
                        GUILayout.ExpandWidth(true));
                }
                
                var elementRect = GUILayoutUtility.GetLastRect();
                if (e.type == EventType.MouseMove && elementRect.Contains(e.mousePosition))
                    selectedIndex = i;

                /* rewrite option when mouse down */
                if (e.type == EventType.MouseDown && elementRect.Contains(e.mousePosition))
                {
                    this.Callback(filteredOptions[i]);
                    editorWindow.Close();
                }
                
                
                if ( selectedIndex == i )
                {
                    GUI.color = hoverColor;
                    GUI.DrawTexture(elementRect, EditorGUIUtility.whiteTexture);
                    GUI.color = Color.white;
                }

                GUI.enabled = true;
            }

            if (lastSelectedIndex != selectedIndex)
            {
                shouldRepaint = true;
                lastSelectedIndex = selectedIndex;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif