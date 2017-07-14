using System;
using System.Threading.Tasks;

namespace UnityCore.Threading
{
    public interface IUnityMainThreadDispatcher
    {
        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        Task Enqueue(Action action);
        Task<T> Enqueue<T>(Func<T> func);

        //Func<Task<TGeneric>, TResult> Enqueue<TGeneric, TResult>(Func<TGeneric, TResult> func);
    }
}