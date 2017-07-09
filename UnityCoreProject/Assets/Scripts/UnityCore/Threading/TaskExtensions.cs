using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var mainThreadTask = UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                action(task);
            });
            return mainThreadTask;
        }

        public static Task<TResult> ContinueWithOnMainThread<TResult, TGeneric>(this Task<TGeneric> task, Func<Task<TGeneric>, TResult> continuationFunction)
        {
            var mainThreadTask = UnityMainThreadDispatcher.Instance.Enqueue<TResult>(() =>
            {
                return continuationFunction.Invoke(task);
            });
            return mainThreadTask;
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
