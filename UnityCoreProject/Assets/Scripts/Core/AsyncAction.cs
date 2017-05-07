using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class IAsyncUnityTask<T>
{
    private T _result { get; set; }

    public bool Complete
    {
        get
        {
            return _complete;
        }

        set
        {
            _complete = value;
        }
    }

    private bool _complete = false;
    
    public delegate void AyncUnityTaskCompleteHandler(object sender, AsyncUnityTaskCompleteEventArgs result);
    public AyncUnityTaskCompleteHandler OnAsyncUnityTaskComplete;
    public class AsyncUnityTaskCompleteEventArgs
    {
        public AsyncUnityTaskCompleteEventArgs(T result)
        {
            Result = result;
        }
        public T Result { get; private set; }
    }
    // Initialise in the foreground so we can access variables
    // and interact with the scene
    public abstract void InitialiseInForeground();
    
    // Always a thread, outside the Unity loop
    public abstract void ExecuteInBackground();

    // Hook back into the main unity thread for completion 
    // i.e. set values, and trigger events on the main thread
    public abstract void CompleteInForeground();

    void TriggerOnCompleteInForeground()
    {
        if(OnAsyncUnityTaskComplete != null)
        {
            var args = new AsyncUnityTaskCompleteEventArgs(_result);
            OnAsyncUnityTaskComplete(this, args);
        }
    }

    
}
