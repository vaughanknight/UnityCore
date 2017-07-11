using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThreadTestEditorWindow : EditorWindow
{
    [MenuItem("Window/Editor Thread Test")]
    public static void Menu()
    {
        var window = EditorWindow.GetWindow<ThreadTestEditorWindow>();
        window.Show();
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Run Thread Test"))
        {
            // Once again, no thread weaving done with the UI, 
            // no hidden coroutines, just 'do this please
            // NOTE: Same code as the player runtime, for editor runtime!
            Tester.Test();
        }
    }
}
