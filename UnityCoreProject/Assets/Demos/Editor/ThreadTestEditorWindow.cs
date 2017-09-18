using Castle.Windsor;
using System.Collections;
using System.Collections.Generic;
using UnityCore.Demos;
using UnityEditor;
using UnityEngine;

public class ThreadTestEditorWindow : EditorWindow
{
    private static IWindsorContainer _container;
    private static ITester _tester;

    [MenuItem("Window/Editor Thread Test")]
    public static void Menu()
    {
        Initialise();

        var window = EditorWindow.GetWindow<ThreadTestEditorWindow>();
        window.Show();
    }

    private static void Initialise()
    {
        if (_container == null)
        {
            _container = new WindsorContainer().Install(new ContainerInstaller());
            _tester = _container.Resolve<ITester>();
        }
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Run Thread Test"))
        {
            // Once again, no thread weaving done with the UI, 
            // no hidden coroutines, just 'do this please
            // NOTE: Same code as the player runtime, for editor runtime!
            _tester.Test();
        }
    }
}
