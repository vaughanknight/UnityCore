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
        //ContinueWithOnMainThread(task =>
        //{
        //    Debug.Log("This could be initialisation on the main thread.");
        //}).
        ContinueWith(task =>
        {
            // Jumping off the main thread for a web request
            Debug.Log("Requesting");
            byte[] buffer = new byte[128];
            // Let's do a long running activity
            var parallel = Parallel.For(0, 9, number =>
            {
                try
                {
                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("http://unsplash.it?" + DateTime.Now.Millisecond);
                    var response = (HttpWebResponse)wr.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Debug.Log("Status:" + response.StatusCode);
                    }
                    else
                    {
                        var stream = response.GetResponseStream();
                        buffer = new byte[response.ContentLength];
                        stream.Read(buffer, 0, (int)response.ContentLength);
                        stream.Close();
                        Debug.Log("Requested");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Exception:" + e.Message);
                }
            });
            Debug.Log("Requesting2");

            return buffer;
        }).
        ContinueWithOnMainThread(task =>
        {
                Debug.Log("wow " + Application.isEditor);
                return 123;
        }).
        ContinueWith(task =>
        {
            // Now we have the texture, loaded from the web request, off the main thread
            // Texture2D.width can only be called on the main thread, so it should fail
            Debug.Log("This is the image width: " + task.Result);
        });
    }

    private static TResult WhatTheFunctionLooksLike<TPrevious, TResult>(Task<TPrevious> task, Func<Task<TPrevious>, TResult> func)
    {
        var t = UnityMainThreadDispatcher.Instance.Enqueue(() => func.Invoke(task));
        t.Wait();
        return t.Result;
    }
}