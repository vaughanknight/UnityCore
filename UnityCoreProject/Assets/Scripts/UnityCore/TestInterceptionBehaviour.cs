using Castle.DynamicProxy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Net;
using System.IO;

public class TestInterceptionBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var t = Task.Factory.StartNew<string>(() =>
        {
            WebRequest web = WebRequest.Create("http://www.microsoft.com");
            var response = web.GetResponse();
            var dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            
            // Read the content.
            string stringResponse = reader.ReadToEnd();

            return stringResponse;
        });
        t.Wait();
        Debug.Log(t.Result);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
