using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityCore.Threading;
using UnityEngine;

public class ThreadTestBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
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
        // First Task runs off the main thread
        Task.Factory.StartNew(() =>
        {
            Debug.Log("This isn't on the main thread");
        }).
        ContinueWith(task =>
        {
            // Still off the main thread for some async
            // i.e. imagine we retrieved some json 
            Debug.Log("Neither is this");
            return "{answer: 42}";
        }).
        ContinueWithOnMainThread(task =>
        {
            // Now jumping onto the main thread 
            // and we have the result from the previous thread!
            Debug.Log(task.Result);

            // And we can do main thread only tasks
            Debug.Log(Application.isEditor);

            // And we can also pass values off the main thread
            return 123;
        }).
        ContinueWith(task =>
        {
            // And voila, we now have the result from the main thread, 
            // back off and can work async
            Debug.Log(task.Result);
        });
    }

}