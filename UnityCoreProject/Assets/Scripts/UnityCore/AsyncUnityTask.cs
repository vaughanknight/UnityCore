using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAsyncUnityTask
{
    void InitialiseInForeground();

    // Always a thread, outside the Unity loop
    void ExecuteInBackground();

    // Hook back into the main unity thread for completion 
    // i.e. set values, and trigger events on the main thread
    void CompleteInForeground();

    bool IsRunning();
}

public enum AsyncUnityTaskState
{
    PreInitialisation,
    Initialising,
    PreExecute,
    Executing,
    PreComplete,
    Completed
}

public class AsyncUnityTaskRunner
{
    private ulong _id;
    private AsyncUnityTaskState _state;
    private object _result;
    private bool _complete = false;
    private IAsyncUnityTask _task;

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
    public object Result
    {
        get
        {
            return _result;
        }

        protected set
        {
            _result = value;
        }
    }
    public ulong Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }
    public AsyncUnityTaskState State
    {
        get
        {
            return _state;
        }

        set
        {
            _state = value;
        }
    }

    public IAsyncUnityTask Task
    {
        get
        {
            return _task;
        }

        set
        {
            _task = value;
        }
    }

    public bool Running
    {
        get
        {
            return _running;
        }

        set
        {
            _running = value;
        }
    }

    public AsyncUnityTaskRunner(IAsyncUnityTask task)
    {
        Task = task;
        Running = false;
    }

    public delegate void AyncUnityTaskCompleteHandler(AsyncUnityTaskRunner sender, AsyncUnityTaskCompleteEventArgs result);
    public AyncUnityTaskCompleteHandler OnAsyncUnityTaskComplete;
    public class AsyncUnityTaskCompleteEventArgs
    {
        public AsyncUnityTaskCompleteEventArgs(object result)
        {
            Result = result;
        }
        public object Result { get; private set; }
    }

    public delegate void AyncUnityTaskInitialisationCompleteHandler(AsyncUnityTaskRunner sender);
    public AyncUnityTaskInitialisationCompleteHandler OnAsyncUnityTaskInitialisationComplete;
    //public class AyncUnityTaskInitialisationCompleteArgs
    //{   
    //    public AyncUnityTaskInitialisationCompleteArgs(object result)
    //    {
    //        Result = result;
    //    }
    //    public object Result { get; private set; }
    //}
    

    private bool _running;

    public void Run()
    {
        if(!Running)
        {
            Running = true;
        }
        else
        {
            throw new System.InvalidOperationException("AsyncUnityTask is already running.");
        }
    }

    void TriggerOnCompleteInForeground()
    {
        if(OnAsyncUnityTaskComplete != null)
        {
            var args = new AsyncUnityTaskCompleteEventArgs(Result);
            OnAsyncUnityTaskComplete(this, args);
        }
    }
}
