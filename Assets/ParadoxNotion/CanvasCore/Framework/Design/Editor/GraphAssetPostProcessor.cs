#if UNITY_EDITOR

using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Editor
{
    ///<summary>Handles post processing of graph assets</summary>
    public class GraphAssetPostProcessor
    {

        [InitializeOnLoadMethod]
        static void PreInit() {
            EditorApplication.delayCall -= Init;
            EditorApplication.delayCall += Init;
        }

        static void Init() {
            //we track graph assets so that we can access them on a diff thread
            AssetTracker.BeginTrackingAssetsOfType(typeof(Graph));
            ParadoxNotion.Design.AssetTracker.onAssetsImported -= OnAssetsImported;
            ParadoxNotion.Design.AssetTracker.onAssetsImported += OnAssetsImported;
        }

        static void OnAssetsImported(string[] paths) {
            foreach ( var path in paths ) {
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.TextAsset));
                if ( asset is UnityEngine.TextAsset ) {
                    var textAsset = (UnityEngine.TextAsset)asset;
                    foreach ( var pair in AssetTracker.trackedAssets ) {
                        if ( pair.Value is Graph && ( pair.Value as Graph ).externalSerializationFile == asset ) {
                            ( pair.Value as Graph ).Deserialize(textAsset.text, null, true);
                            break;
                        }
                    }
                }
            }
        }
    }
}

#endif