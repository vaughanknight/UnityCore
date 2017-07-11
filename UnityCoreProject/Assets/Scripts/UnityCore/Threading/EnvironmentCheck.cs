using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnvironmentCheck  {
    private bool _isEditor = false;
    
    public bool IsEditor
    {
        get
        {
            return _isEditor;
        }

        private set
        {
            _isEditor = value;
        }
    }
}
