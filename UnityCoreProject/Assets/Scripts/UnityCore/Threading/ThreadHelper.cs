using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCore.Threading
{
    public static class ThreadHelper
    {
        public static void Update(Queue<Action> _executionQueue)
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        public static Task Enqueue(Queue<Action> _executionQueue, Action action)
        {
            lock (_executionQueue)
            {
                var task = new Task(action);
                _executionQueue.Enqueue(() =>
                {
                    task.RunSynchronously();
                });
                return task;
            }
        }
        
        public static Task<T> Enqueue<T>(Queue<Action> _executionQueue, Func<T> func)
        {
            lock (_executionQueue)
            {
                var task = new Task<T>(func);
                _executionQueue.Enqueue(() =>
                {
                    task.RunSynchronously();
                });
                return task;
            }
        }
    }
}
