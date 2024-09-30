#if UNITY_EDITOR

using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor
{

    [CustomEditor(typeof(Graph), true)]
    public class GraphInspector : UnityEditor.Editor
    {

        private Graph graph => (Graph)target;

        public override void OnInspectorGUI() {
            UndoUtility.CheckUndo(this, "Graph Inspector");
            ShowBasicGUI();
            EditorUtils.Separator();

            if ( graph.externalSerializationFile == null ) {
                if ( GUILayout.Button("Create External Serialization Text Asset") ) {
                    var path = EditorUtility.SaveFilePanelInProject("Create Text Asset", target.name, "txt", "");
                    if ( !string.IsNullOrEmpty(path) ) {
                        System.IO.File.WriteAllText(path, ParadoxNotion.Serialization.JSONSerializer.PrettifyJson(graph.GetSerializedJsonData()));
                        AssetDatabase.Refresh();
                        graph.externalSerializationFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                    }
                }
            } else {
                graph.externalSerializationFile = (TextAsset)EditorGUILayout.ObjectField("External Serialization File", graph.externalSerializationFile, typeof(TextAsset), true);
                EditorGUILayout.HelpBox("Be careful! The assigned Text Asset contents will be completely replaced with the json serialization of this graph. The graph will also deserialize from the json contents of the Text Asset whenever the Text Asset is imported by Unity. You can remove the assigned file at any time.", MessageType.Warning);
            }

            EditorUtils.Separator();
            BlackboardEditor.ShowVariables(graph.blackboard, graph);
            EditorUtils.EndOfInspector();
            UndoUtility.CheckDirty(this);
        }

        //name, description, edit button
        void ShowBasicGUI() {
            GUILayout.Space(10);
            graph.category = GUILayout.TextField(graph.category);
            EditorUtils.CommentLastTextField(graph.category, "Category...");

            graph.comments = GUILayout.TextArea(graph.comments, GUILayout.Height(45));
            EditorUtils.CommentLastTextField(graph.comments, "Comments...");

            GUI.backgroundColor = Colors.lightBlue;
            if ( GUILayout.Button(string.Format("EDIT {0}", graph.GetType().Name.SplitCamelCase().ToUpper())) ) {
                GraphEditor.OpenWindow(graph);
            }
            GUI.backgroundColor = Color.white;
        }
    }
}

#endif