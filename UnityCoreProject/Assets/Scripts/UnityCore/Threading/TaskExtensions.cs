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
            return HandToThreadAndWait<TResult>(task, func, UnityMainThreadDispatcher.Instance);
        }

        private static TResult HandToMainThreadAndWait<TPrevious, TResult>(Task<TPrevious> task, Func<Task<TPrevious>, TResult> func)
        {
            return HandToThreadAndWait(task, func, UnityMainThreadDispatcher.Instance);
        }

        private static void HandToMainThreadAndWait<TPrevious>(Task<TPrevious> task, Action<Task<TPrevious>> action)
        {
            HandToThreadAndWait(task, action, UnityMainThreadDispatcher.Instance);
        }
        
        private static void HandToMainThreadAndWait(Task task, Action<Task> action)
        {
            HandToThreadAndWait(task, action, UnityMainThreadDispatcher.Instance);
        }

        private static TResult HandToThreadAndWait<TResult>(Task task, Func<Task, TResult> func, IUnityMainThreadDispatcher dispatcher)
        {
            var t = dispatcher.Enqueue(() => func.Invoke(task));
            t.Wait();
            return t.Result;
        }

        private static TResult HandToThreadAndWait<TPrevious, TResult>(Task<TPrevious> task, Func<Task<TPrevious>, TResult> func, IUnityMainThreadDispatcher dispatcher)
        {
            var t = dispatcher.Enqueue(() => func.Invoke(task));
            t.Wait();
            return t.Result;
        }

        /// <summary>
        /// General methods
        /// </summary>
        /// <param name="task">The previous task</param>
        /// <param name="action">The action to perform</param>
        /// <param name="dispatcher">The Unity Dispatcher to run with</param>
        public static void HandToThreadAndWait(Task task, Action<Task> action, IUnityMainThreadDispatcher dispatcher)
        {
            var t = dispatcher.Enqueue(() => action(task));
            t.Wait();
        }

        public static void HandToThreadAndWait<TPrevious>(Task<TPrevious> task, Action<Task<TPrevious>> action, IUnityMainThreadDispatcher dispatcher)
        {
            var t = dispatcher.Enqueue(() => action(task));
            t.Wait();
        }
    }
}
