using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UnityCore;
using UnityEditor;
using UnityEngine;

public class NugetForUnityWindow : EditorWindow {

    private bool Verbose = false;

    [MenuItem("Nuget/Configure Nuget for Unity")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (NugetForUnityWindow)EditorWindow.GetWindow(typeof(NugetForUnityWindow));
        window.titleContent = new GUIContent("Nuget for Unity");
        window.maxSize = new Vector2(406f, 506f);
        window.minSize = window.maxSize;
        window.wantsMouseMove = true;
        window.wantsMouseEnterLeaveWindow = true;
        window.Show();
    }

    public class Icons : Singleton<Icons>
    {
        public Texture2D NugetReadyIcon { get; private set; }
        public Texture2D NugetNotReadyIcon { get; private set; }
        public Texture2D Open { get; private set; }
        public Texture2D Install { get; private set; }
        public Texture2D InstallHover { get; private set; }
        public Texture2D InstallDown { get; private set; }

        public Icons()
        {
            NugetReadyIcon = Load("nuget_ready.png");
            NugetNotReadyIcon = Load("nuget_not_ready.png");
            Open = Load("button_open.png");
            Install = Load("install.png");
            InstallHover = Load("install_hover.png");
            InstallDown = Load("install_down.png");
        }
        private const string BASE_FOLDER = "Assets/Scripts/NugetForUnity/editor/Media/";
        public Texture2D Load(string fileName)
        {
            return (Texture2D)EditorGUIUtility.Load(BASE_FOLDER + fileName);
        }
    }
    
    public class Styles : Singleton<Styles>
    {
        public GUIStyle RichTextStyle {get; private set;}
        public Styles()
        {
            RichTextStyle = new GUIStyle();
            RichTextStyle.richText = true;
            RichTextStyle.wordWrap = true;
        }
    }

    public void Awake()
    {
        InitialisePaths();

        RunChecks();
    }

    private void RunChecks()
    {
        CheckSolutionAsync().
            ContinueWith(task =>
            {
                if (_solutionExists)
                {
                    CheckSolutionIncludesProjectAsync();
                }
            });
        CheckNugetConfigAsync();
        CheckNugetProjectAsync();
    }

    private void InitialisePaths()
    {
        var dataPath = Application.dataPath;
        var pathTokens = dataPath.Split("/"[0]);
        _projectName = pathTokens[pathTokens.Length - 2];
        _solutionName = _projectName + ".sln";
    }

    private const string NUGET_CONFIG = "Nuget.Config";
    private const string NUGET_CONFIG_SOURCE_FILE = "Assets/Scripts/NugetForUnity/Nuget.Config";

    private const string NUGET_PROJECT_PATH = "NugetForUnity/NugetForUnity.csproj";
    private const string NUGET_PROJECT_TEMPLATE_PATH = "Assets/Scripts/NugetForUnity/csproj.xml";

    private const string NUGET_PROJECT_FOLDER = "NugetForUnity";

    private const string TEXT_TITLE = "<size=20>Nuget for Unity</size>";
    private const string TEXT_SOLUTION_EXISTS_FORMAT = "<b>{0}</b> solution exists.";
    private const string TEXT_SOLUTION_DOESNT_EXIST_FORMAT = "<b>{0}</b> solution does not exist.";
    private const string TEXT_NUGET_PROJECT_EXISTS = "<b>NugetForUnity</b> project exists.";
    private const string TEXT_NUGET_PROJECT_DOESNT_EXIST = "<b>NugetForUnity</b> project does not exist.";
    private const string TEXT_BUTTON_CREATE_NUGET_PROJECT = "Create Nuget for Unity Project";
    private const string TEXT_NUGET_CONFIG_EXISTS = "<b>Nuget.Config</b> exists.";
    private const string TEXT_NUGET_CONFIG_DOESNT_EXIST = "<b>Nuget.Config</b> does not exist.";
    private const string TEXT_BUTTON_CREATE_NUGET_CONFIG = "Create Nuget.Config";
    private const string TEXT_SOLUTION_FILE_EXISTS_FORMAT = "Solution file <b>{0}</b> exists.";
    private const string TEXT_SOLUTION_FILE_DOESNT_EXIST = "Solution file <b>{0}</b> does not exist.";
    private const string TEXT_BUTTON_CHECK_SOLUTION = "Check solution files.";

    private bool _solutionExists = false;
    private bool _solutionHasNugetProject = false;
    private string _solutionName = "";
    private string _projectName;

    private Task CheckSolutionAsync()
    {
        return Task.Factory.StartNew(() => CheckSolution());
    }

    private void CheckSolution()
    {
        if (File.Exists(_solutionName))
        {
            if (Verbose) Debug.LogFormat(TEXT_SOLUTION_EXISTS_FORMAT, _solutionName);
            _solutionExists = true;
        }
        else
        {
            if (Verbose) Debug.LogFormat(TEXT_SOLUTION_DOESNT_EXIST_FORMAT, _solutionName);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
       
        GUILayout.Space(10);

        InformationUI();
        GUILayout.Space(10);

        GUILayout.EndVertical();
    }

    private void CheckSolutionIncludesProject()
    {
        string slnFileText = GetSolutionFileContent();
        _solutionHasNugetProject = slnFileText.Contains("NugetForUnity.csproj");
    }

    private string GetSolutionFileContent()
    {
        return File.ReadAllText(_solutionName);
    }

    private Task CheckSolutionIncludesProjectAsync()
    {
        return Task.Factory.StartNew(() => CheckSolutionIncludesProject());
    }

    private void UpdateSolutionFile()
    {
        Debug.Log("Updating solution file.");
        // Do this synchronous
        CheckSolutionIncludesProject();

        // Still only update if required - no need to add twice
        if (!_solutionHasNugetProject)
        {
            var slnFileText = GetSolutionFileContent();
            if (Verbose) Debug.Log("Updating solution.");
            var index = slnFileText.IndexOf("Project(");
            slnFileText = slnFileText.Insert(index, "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"NugetForUnity\", \"NugetForUnity\\NugetForUnity.csproj\", \"{48D3AFDD-DDEA-4A45-8CA0-D911E9020B20}\"\nEndProject\n");
            File.WriteAllText(_solutionName, slnFileText);

            CheckSolutionIncludesProject();
        }
        else
        {
            if (Verbose) Debug.Log("Solution already updated.");
        }
    }

    private bool _hover = false;

    private void InformationUI()
    {
        if (_nugetProjectExists && _nugetConfigExists && _solutionExists && _solutionHasNugetProject)
        {
            GUILayout.Box(Icons.Instance.NugetReadyIcon, GUILayout.Width(400), GUILayout.Height(500));
            //GUI.Button(new Rect(100, 400, 200, 50), "Open");
        }
        else
        {
            GUILayout.Box(Icons.Instance.NugetNotReadyIcon, GUILayout.Width(400), GUILayout.Height(500));

            if(GUI.Button(new Rect(100, 400, 200, 50), "Install"))
            {
                MakeNugetReady();
            }
        }
    }

    private void MakeNugetReady()
    {
        if(Verbose)
        { 
            Debug.Log("_nugetProjectExists " + _nugetProjectExists);
            Debug.Log("_nugetConfigExists " + _nugetConfigExists);
            Debug.Log("_solutionExists " + _solutionExists);
            Debug.Log("_solutionHasNugetProject " + _solutionHasNugetProject);
        }

        if (_solutionExists)
        {
            Debug.Log("Updating project.");
            CreateNugetConfigAsync().
            ContinueWith(task => CreateNugetProject()).
            ContinueWith(task => UpdateSolutionFile()).
            ContinueWith(task => RunChecks());
        }
    }

    private bool _nugetProjectExists = false;
    private void NugetProjectUI()
    {
        if (_nugetProjectExists)
        {
            GUILayout.Label(TEXT_NUGET_PROJECT_EXISTS, Styles.Instance.RichTextStyle);
        }
        else
        {
            GUILayout.Label(TEXT_NUGET_PROJECT_DOESNT_EXIST, Styles.Instance.RichTextStyle);
            if (GUILayout.Button(TEXT_BUTTON_CREATE_NUGET_PROJECT))
            {
                CreateNugetProject();
            }
        }  
    }

    private Task CheckNugetProjectAsync()
    {
        return Task.Factory.StartNew(() => CheckNugetProject());
    }

    private void CheckNugetProject()
    {
        _nugetProjectExists = File.Exists(NUGET_PROJECT_PATH);
    }


    private Task CreateNugetProjectAsync()
    {
        return Task.Factory.StartNew(() => CreateNugetProject());
    }

    private void CreateNugetProject()
    {
        Debug.Log("Creating NugetForUnity.");
        // Only create the directory if it doesn't exist
        if (!Directory.Exists(NUGET_PROJECT_FOLDER))
        {
            Directory.CreateDirectory(NUGET_PROJECT_FOLDER);
        }

        // Overwrite the file if it exists
        var text = File.ReadAllText(NUGET_PROJECT_TEMPLATE_PATH);
        var guid = Guid.NewGuid().ToString();
        var finalText = String.Format(text, guid);
        File.WriteAllText(NUGET_PROJECT_PATH, finalText);
        
        CheckNugetProject();
    }

    private bool _nugetConfigExists = false;
    private void NugetConfigUI()
    {
        if (_nugetConfigExists)
        {
            GUILayout.Label(TEXT_NUGET_CONFIG_EXISTS, Styles.Instance.RichTextStyle);
        }
        else
        {
            GUILayout.Label(TEXT_NUGET_CONFIG_DOESNT_EXIST, Styles.Instance.RichTextStyle);
            if (GUILayout.Button(TEXT_BUTTON_CREATE_NUGET_CONFIG))
            {
                CreateNugetConfig();
            }
        }
    }

    private Task CreateNugetConfigAsync()
    {
        return Task.Factory.StartNew(() => CreateNugetConfig());
    }

    private void CreateNugetConfig()
    {
        Debug.Log("Creating Nuget Config.");
        File.Copy(NUGET_CONFIG_SOURCE_FILE, NUGET_CONFIG);
        CheckNugetConfig();
    }

    private Task CheckNugetConfigAsync()
    {
        return Task.Factory.StartNew(() => CheckNugetConfig());
    }

    private void CheckNugetConfig()
    {
        _nugetConfigExists = File.Exists(NUGET_CONFIG);
    }

    private void SolutionSummaryUI()
    {
        if (_solutionExists)
        {
            GUILayout.Label(String.Format(TEXT_SOLUTION_FILE_EXISTS_FORMAT, _solutionName), Styles.Instance.RichTextStyle);
        }
        else
        {
            GUILayout.Label(String.Format(TEXT_SOLUTION_FILE_DOESNT_EXIST, _solutionName), Styles.Instance.RichTextStyle);
            if (GUILayout.Button(TEXT_BUTTON_CHECK_SOLUTION))
            {
                CheckSolution();
            }
        }
       
    }
}
