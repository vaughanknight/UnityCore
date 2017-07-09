using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCore.Threading
{
    /// <summary>
    /// This thread dispatcher is for the main thread in game.  
    /// </summary>
    public class UnityGameThreadDispatcher : SelfInstantiatingSingletonBehaviour<UnityGameThreadDispatcher>, IUnityMainThreadDispatcher
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        public void Update()
        {
            ThreadHelper.Update(_executionQueue);
        }
        public Task Enqueue(Action action)
        {
            return ThreadHelper.Enqueue(_executionQueue, action);
        }
        public Task<T> Enqueue<T>(Func<T> func)
        {
            return ThreadHelper.Enqueue<T>(_executionQueue, func);
        }

       
    }
}
