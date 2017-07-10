﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCore.Threading
{
    /// <summary>
    /// These extensions are for the Task.Parallel library for .Net 3.5.  It adds main thread
    /// support independent of running in the editor or in the player.
    /// </summary>
    public static class TaskExtensions
    {
        // Input void, output void
        public static Task ContinueWithOnMainThread(this Task task, Action<Task> action)
        {
            return task.ContinueWith(t =>
            {
                HandToMainThreadAndWait(task, action);
            });
        }

        // Input void, output result
        public static Task<TResult> ContinueWithOnMainThread<TResult>(this Task task, Func<Task, TResult> continuationFunction)
        {
            return task.ContinueWith(t =>
            {
                return HandToMainThreadAndWait(task, continuationFunction);
            });
        }

        // Input value, output result
        public static Task<TResult> ContinueWithOnMainThread<TResult, TGeneric>(this Task<TGeneric> task, Func<Task<TGeneric>, TResult> continuationFunction)
        {
            return task.ContinueWith( t=>
            {
                return HandToMainThreadAndWait(task, continuationFunction);
            });
        }

        // Has result, takes nothing
        private static TResult HandToMainThreadAndWait<TResult>(Task task, Func<Task, TResult> func)
        {
            var t = UnityMainThreadDispatcher.Instance.Enqueue(() => func.Invoke(task));
            t.Wait();
            return t.Result;
        }

        private static TResult HandToMainThreadAndWait<TPrevious, TResult>(Task<TPrevious> task, Func<Task<TPrevious>, TResult> func)
        {
            var t = UnityMainThreadDispatcher.Instance.Enqueue(() => func.Invoke(task));
            t.Wait();
            return t.Result;
        }

        private static void HandToMainThreadAndWait<TPrevious>(Task<TPrevious> task, Action<Task<TPrevious>> action)
        {
            var t = UnityMainThreadDispatcher.Instance.Enqueue(() => action(task));
            t.Wait();
        }
        
        private static void HandToMainThreadAndWait(Task task, Action<Task> action)
        {
            var t = UnityMainThreadDispatcher.Instance.Enqueue(() => action(task));
            t.Wait();
        }
    }
}
