using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityCore.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class ThreadTest : EditorWindow {

    [MenuItem("Nuget/Thread")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (ThreadTest)EditorWindow.GetWindow(typeof(ThreadTest));
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();

        if(GUILayout.Button("Test Threads"))
        {
            Tester.Test();
        }
        GUILayout.EndVertical();
    }
}
