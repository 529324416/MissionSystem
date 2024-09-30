﻿#if UNITY_EDITOR

using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor
{

    public static class Commands
    {

        ///----------------------------------------------------------------------------------------------

        [UnityEditor.InitializeOnLoadMethod]
        public static void SetNodeCanvasDefine() {
            DefinesManager.SetDefineActiveForCurrentTargetGroup("NODECANVAS", true);
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Assets/Create/ParadoxNotion/NodeCanvas/New Task")]
        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/New Task", false, 90)]
        public static void ShowTaskWizard() {
            TaskWizardWindow.ShowWindow();
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Global Scene Blackboard", false, 10)]
        public static void CreateGlobalSceneBlackboard() {
            Selection.activeObject = GlobalBlackboard.Create();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Preferred Types Editor")]
        public static void ShowPrefTypes() {
            TypePrefsEditorWindow.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Console")]
        public static void OpenConsole() {
            GraphConsole.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Explorer")]
        public static void OpenExplorer() {
            GraphExplorer.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Refactor")]
        public static void OpenRefactor() {
            GraphRefactor.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Active Owners Overview")]
        public static void OpenOwnersOverview() {
            ActiveOwnersOverview.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/External Inspector Panel")]
        public static void ShowExternalInspector() {
            ExternalInspectorWindow.ShowWindow();
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Welcome Window")]
        public static void ShowWelcome() {
            WelcomeWindow.ShowWindow(typeof(NodeCanvas.BehaviourTrees.BehaviourTree));
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Website...")]
        public static void VisitWebsite() {
            Help.BrowseURL("https://nodecanvas.paradoxnotion.com");
        }
    }
}

#endif