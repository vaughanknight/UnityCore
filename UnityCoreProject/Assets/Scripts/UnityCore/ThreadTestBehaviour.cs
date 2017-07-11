using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityCore.Threading;
using UnityEditor;
//using UnityCore.Threading;

using UnityEngine;

public class ThreadTestBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start ()
    { 
        
        Tester.Test();
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class Tester
{
    public static void Test()
    {

        UnityMainThreadDispatcher.Initialise();

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