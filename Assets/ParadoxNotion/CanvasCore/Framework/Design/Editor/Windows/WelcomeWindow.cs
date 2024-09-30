#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion;
using UnityEngine.Networking;

namespace NodeCanvas.Editor
{

    public class WelcomeWindow : EditorWindow
    {

        private static System.Type assetType;
        private Texture2D header;
        private Texture2D docsIcon;
        private Texture2D resourcesIcon;
        private Texture2D supportIcon;
        private Texture2D communityIcon;

        private GraphInfoAttribute att;
        private string packageName;
        private string docsURL;
        private string resourcesURL;
        private string forumsURL;
        private string discordUrl = "https://discord.gg/97q2Rjh";

        private string webMessage;

        //...
        public static void ShowWindow(System.Type t) {
            assetType = t;
            var window = CreateInstance<WelcomeWindow>();
            window.ShowUtility();
        }

        //...
        void OnEnable() {
            titleContent = new GUIContent("Welcome");
            if ( assetType == null ) { assetType = GraphEditor.currentGraph?.GetType(); }
            if ( assetType == null ) { assetType = ReflectionTools.GetImplementationsOf(typeof(Graph)).Where(x => x.IsDefined(typeof(GraphInfoAttribute), true)).LastOrDefault(); }
            att = assetType != null ? assetType.RTGetAttributesRecursive<GraphInfoAttribute>().LastOrDefault() : null;

            packageName = att != null ? att.packageName : "NodeCanvas";
            docsURL = att != null ? att.docsURL : "https://paradoxnotion.com/";
            resourcesURL = att != null ? att.resourcesURL : "https://paradoxnotion.com/";
            forumsURL = att != null ? att.forumsURL : "https://paradoxnotion.com/";

            header = Resources.Load(string.Format("{0}Header", packageName)) as Texture2D;
            docsIcon = Resources.Load("Manual") as Texture2D;
            resourcesIcon = Resources.Load("Resources") as Texture2D;
            supportIcon = Resources.Load("Support") as Texture2D;
            communityIcon = Resources.Load("Community") as Texture2D;
            var size = new Vector2(header != null ? header.width : 800, 435);
            minSize = size;
            maxSize = size;
            FetchWebMessageBoard();
        }

        //...
        void FetchWebMessageBoard() {
            var url = "https://paradoxnotion.com/files/softwaremessageboard.txt";
            var request = UnityWebRequest.Get(url);
            var op = request.SendWebRequest();
            op.completed += (x) =>
            {
                webMessage = request.downloadHandler?.text;
                if ( !string.IsNullOrEmpty(webMessage) ) {
                    var result = string.Empty;
                    var boards = webMessage.Split('|');
                    foreach ( var board in boards ) {
                        var targetPair = board.GetStringWithinOuter('<', '>').Split(':');
                        var target = targetPair[0];
                        var isAll = target.ToLower() == "all";
                        if ( isAll || target.ToLower() == packageName.ToLower() ) {
                            if ( !isAll ) {
                                var version = targetPair[1];
                                var uptodate = NodeCanvas.Framework.Internal.GraphSource.FRAMEWORK_VERSION == float.Parse(version);
                                result += uptodate ? "<b>You are up to date on the latest version!</b>" : string.Format("<b>There is a new version available! ( v{0} )</b>", version);
                            }
                            var content = board.GetStringWithinOuter('{', '}');
                            result += content.Replace("\t", "").TrimEnd();
                        }
                    }
                    webMessage = result.Trim();
                }
                request.Dispose();
                Repaint();
            };
        }


        //...
        void OnGUI() {

            GUI.skin.label.richText = true;

            var headerRect = header != null ? new Rect(0, 0, header.width, header.height) : new Rect(0, 0, maxSize.x, 110);
            EditorGUIUtility.AddCursorRect(headerRect, MouseCursor.Link);
            if ( GUI.Button(headerRect, string.Empty, GUIStyle.none) ) {
                UnityEditor.Help.BrowseURL("https://paradoxnotion.com");
            }

            if ( header != null ) {
                GUI.DrawTexture(headerRect, header);
            } else {
                Styles.Draw(headerRect, Styles.roundedBox);
                GUI.Label(headerRect, $"\t\t<size=30><b>{packageName}</b></size>", Styles.leftLabel);
            }

            var copyrightText = "<color=#9c9c9c><size=10><b>© 2014-2024 Paradox Notion. All rights reserved.</b></size></color>";
            var size = Styles.leftLabel.CalcSize(new GUIContent(copyrightText));
            var copyrightRect = new Rect(92, 69, size.x, size.y);
            GUI.color = Color.black.WithAlpha(0.05f);
            GUI.DrawTexture(copyrightRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(copyrightRect, copyrightText, Styles.topLeftLabel);

            GUILayout.Space(headerRect.height);

            GUI.Label(new Rect(headerRect.x + 333, headerRect.yMax - 56, 150, 20), $"<color=#d2d2d2><size=10><b>v{NodeCanvas.Framework.Internal.GraphSource.FRAMEWORK_VERSION}</b></size></color>");

            ///----------------------------------------------------------------------------------------------

            if ( !string.IsNullOrEmpty(webMessage) ) {
                var messageRect = Rect.MinMaxRect(headerRect.xMax - 385, headerRect.yMin + 5, headerRect.xMax - 5, headerRect.yMax - 5);
                GUI.color = Color.white.WithAlpha(0.5f);
                Styles.Draw(messageRect, Styles.roundedBox);
                GUI.color = Color.white;
                GUI.Label(messageRect.ExpandBy(-5), $"<color=#d2d2d2>{webMessage}</color>", Styles.topLeftLabel);
            }

            ///----------------------------------------------------------------------------------------------

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            GUILayout.Space(10);

            var titleRect = headerRect;
            titleRect.x += 30;

            GUILayout.Label(string.Format("Welcome and thank you for purchasing {0}! Following are a few important links to get you started:", packageName));
            GUILayout.Space(10);

            ///----------------------------------------------------------------------------------------------

            ShowEntry(docsIcon, "<size=16><b>Documentation</b></size>\nRead thorough documentation and API reference online.", docsURL);
            ShowEntry(resourcesIcon, "<size=16><b>Resources</b></size>\nDownload samples, extensions and other resources.", resourcesURL);
            ShowEntry(supportIcon, "<size=16><b>Support</b></size>\nJoin the online forums, get support and give feedback.", forumsURL);
            ShowEntry(communityIcon, "<size=16><b>Community</b></size>\nJoin the online Discord community.", discordUrl);


            ///----------------------------------------------------------------------------------------------

            GUILayout.FlexibleSpace();

            Prefs.hideWelcomeWindow = EditorGUILayout.ToggleLeft("Don't show again.", Prefs.hideWelcomeWindow);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);
        }

        //...
        void ShowEntry(Texture2D icon, string text, string url) {
            GUILayout.BeginHorizontal(Styles.roundedBox);
            GUI.backgroundColor = Color.clear;
            GUI.contentColor = EditorGUIUtility.isProSkin ? ColorUtils.Grey(0.8f) : Color.black;
            if ( GUILayout.Button(icon, GUILayout.Width(50), GUILayout.Height(50)) ) {
                UnityEditor.Help.BrowseURL(url);
            }
            GUI.contentColor = Color.white;
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.Label(text);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }
    }
}

#endif