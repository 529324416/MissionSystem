using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace ParadoxNotion.Services
{

    ///<summary>Simple Thread helper for both runtime and editor</summary>
    public static class Threader
    {

#if UNITY_EDITOR
        //this is to be able to call isPlaying in other threads
        [UnityEditor.InitializeOnLoadMethod]
#if UNITY_2019_3_OR_NEWER
        //the 2nd attribute is used for 'no domain reload'
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
#endif        
        static void Init() {
            applicationIsPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeChanged;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeChanged;
        }
        static void PlayModeChanged(UnityEditor.PlayModeStateChange state) {
            if ( state == UnityEditor.PlayModeStateChange.EnteredPlayMode ) { applicationIsPlaying = true; }
            if ( state == UnityEditor.PlayModeStateChange.ExitingPlayMode ) { applicationIsPlaying = false; }
        }

#else

        static Threader() { applicationIsPlaying = true; }
#endif


        public static bool applicationIsPlaying { get; private set; }
        public static bool isMainThread => Thread.CurrentThread.ManagedThreadId == 1;


        ///----------------------------------------------------------------------------------------------

        public static Thread StartAction(Thread thread, Action function, Action callback = null) {
            if ( thread != null && thread.IsAlive ) { thread.Abort(); }
            thread = new Thread(() => function());
            Begin(thread, callback);
            return thread;
        }

        ///----------------------------------------------------------------------------------------------

        public static Thread StartFunction<TResult>(Thread thread, Func<TResult> function, Action<TResult> callback = null) {
            if ( thread != null && thread.IsAlive ) thread.Abort();
            TResult result = default(TResult);
            thread = new Thread(() => { result = function(); });
            Begin(thread, () => { callback(result); });
            return thread;
        }

        ///----------------------------------------------------------------------------------------------

        //This intermediate method exists to seperate editor and runtime usage.
        static void Begin(Thread thread, Action callback) {

            thread.Start();

#if UNITY_EDITOR
            if ( !applicationIsPlaying ) {
                threadMonitors.Add(ThreadMonitor(thread, callback));
                return;
            }
#endif

            MonoManager.current.StartCoroutine(ThreadMonitor(thread, callback));
        }

        ///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private static List<IEnumerator> threadMonitors = new List<IEnumerator>();
        [UnityEditor.InitializeOnLoadMethod]
        static void Initialize() {
            UnityEditor.EditorApplication.update += OnEditorUpdate;
        }

        //So that threads work in Editor too
        static void OnEditorUpdate() {
            if ( threadMonitors.Count > 0 ) {
                for ( var i = 0; i < threadMonitors.Count; i++ ) {
                    var e = threadMonitors[i];
                    if ( !e.MoveNext() ) {
                        threadMonitors.RemoveAt(i);
                    }
                }
            }
        }
#endif


        ///----------------------------------------------------------------------------------------------

        //Use IEnumerators and unity coroutines to handle updating the thread.
        static IEnumerator ThreadMonitor(Thread thread, Action callback) {

            while ( thread.IsAlive ) {
                yield return null;
            }

            //This yield is not required.
            //It's for consistency matter when writing code so that we know there will always be at least 1 frame delay.
            yield return null;

            if ( ( thread.ThreadState & ThreadState.AbortRequested ) != ThreadState.AbortRequested ) {
                thread.Join();
                if ( callback != null ) {
                    callback();
                }
            }
        }
    }

}
