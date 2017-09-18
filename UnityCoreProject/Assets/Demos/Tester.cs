using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCore.Threading;
using UnityEngine;


namespace UnityCore.Demos
{
    public interface ITester
    {
        void Test();
    }
    /// <summary>
    /// This is a plain class, that doesn't have to know about coroutines, have
    /// iterators as return types of methods, and weaves back and forth between
    /// the main thread, and off again passive values.
    /// </summary>
    public class Tester : ITester
    {
        public void Test()
        {
            Debug.Log("NOTE: Check the source code of ThreadTestBehaviour in the Demo's folder for more information");

            // We force initialise on classes that can be called from either the 
            // editor or the player due to strange behaviours with threads in
            // the Unity Editor.

            // However, if this code is just for the player, call Initialise once in your 
            // entire game lifecycle up front which ensures it knows the main thread context.
            UnityMainThreadDispatcher.ForceInitialise();

            // First Task runs off the main thread
            Task.Factory.StartNew(() =>
            {
                Debug.Log("Action<Task> off main thread.");
            }).
            ContinueWithOnMainThread(task =>
            {
                Debug.Log("Action<Task> on main thread.");
            }).
            ContinueWith(task =>
            {
                // Still off the main thread for some async
                // i.e. imagine we retrieved some json 
                Debug.Log("Func<Task, String> off the main thread.");
                return "{answer: 42}";
            }).
            ContinueWithOnMainThread(task =>
            {

                // Now jumping with data back onto the main thread 
                Debug.Log("Func<Task<String>, bool> on the main thread argument: " + task.Result);

                // And we can do main thread only tasks
                return Application.isEditor;

            }).
            ContinueWith(task =>
            {
                // And voila, we now have the result from the main thread, 
                // back off and can work async
                Debug.Log("Action<Task<bool>> off the main thread with argument: " + task.Result);
            });
        }
    }
}