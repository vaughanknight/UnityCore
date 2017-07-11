using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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
            UnityThreadHelper.Update(_executionQueue);
        }
        public Task Enqueue(Action action)
        {
            return UnityThreadHelper.Enqueue(_executionQueue, action);
        }

        public Task<T> Enqueue<T>(Func<T> func)
        {
            return UnityThreadHelper.Enqueue<T>(_executionQueue, func);
        }
    }
}
