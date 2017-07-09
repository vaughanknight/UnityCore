using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncUnityTaskManager : SelfInstantiatingSingletonBehaviour<AsyncUnityTaskManager>
{
    private Dictionary<ulong, AsyncUnityTaskRunner> _runningTasks = new Dictionary<ulong, AsyncUnityTaskRunner>();

    private ulong _idCounter = 1;

	public void Run(IAsyncUnityTask task)
    {
        if(task.IsRunning())
        {
            throw new System.InvalidOperationException("Task already running.");
        }
        else
        {
            var runner = new AsyncUnityTaskRunner(task);
            runner.Run();
        }
    }

    private void ExecuteInThread()
    {

    }
}
