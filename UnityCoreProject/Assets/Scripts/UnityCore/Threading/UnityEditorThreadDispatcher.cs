using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace UnityCore.Threading
{
    public class UnityEditorThreadDispatcher : Singleton<UnityEditorThreadDispatcher>, IUnityMainThreadDispatcher
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        public UnityEditorThreadDispatcher()
        {
            EditorApplication.update += Update;
        }
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
