using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SingletonBehaviour ensures that
/// there is only ever a single copy of an inheriting behaviour, that in turn allows Singleton like
/// interactions i.e Singleton.Instance.  It will dstroy any GameObject with subsequent behaviours of  
/// the same type.  Note this is not just deleting the singleton behaviour to stop GameObject leaks.
/// 
/// It will also log an error when a duplicate Singleton Behaviour is created 
/// so you can clean up your project.
/// 
/// Inherits from SmartBehaviour because everything should, even singletons.
/// </summary>
public class SingletonBehaviour<T> : SmartBehaviour where T : SmartBehaviour
{
    private const string TEXT_SINGLETON_EXISTS_ERROR = "Singleton '{0}' is already created. Destroying the object.";
    private static SmartBehaviour _instance;

    public static T Instance
    {
        get
        {
            return (T)_instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            // This is the first instance of the behaviour
            _instance = this;
        }
        else
        {
            // Log an error so developers can clean up their code
            Debug.LogFormat(TEXT_SINGLETON_EXISTS_ERROR, this.GetType().Name);

            // Destroy the entire game object this behaviour is attached to
            GameObject.Destroy(this.gameObject);
        }
    }
}
