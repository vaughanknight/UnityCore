using System;
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
        public static Task ContinueWithOnMainThread(this Task task, Action<Task> action)
        {
            return task.ContinueWith(t =>
            {
                return MainThreadHandoff(task, continuationFunction);
            });
        }

        public static Task<TResult> ContinueWithOnMainThread<TResult, TGeneric>(this Task<TGeneric> task, Func<Task<TGeneric>, TResult> continuationFunction)
        {
            Debug.Log("MainThread Test");
            //var mainThreadTask = UnityMainThreadDispatcher.Instance.Enqueue<TResult>(() =>
            //{
            //    return continuationFunction.Invoke(task);
            //});
            //return mainThreadTask;
            return task.ContinueWith( t=>
            {
                return HandToMainThreadAndWait(task, continuationFunction);
            });
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

        public static Task ContinueWithOnMainThread<TGeneric>(this Task<TGeneric> task, Action<Task<TGeneric>> continuationFunction)
        {
            var mainThreadTask = UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                continuationFunction(task);
            });
            return mainThreadTask;
        }
    }
}
