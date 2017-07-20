using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UnityCore
{
    public class SelfInstantiatingSingletonBehaviour<T> : SingletonBehaviour<T> where T : SmartBehaviour
    {
        public new static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // This is the first instance 
                    var obj = new GameObject(typeof(T).Name);
                    obj.AddComponent<T>();
                }
                return (T)_instance;
            }
        }

        public new void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                // Log an error so developers can clean up their code
                Debug.LogFormat(TEXT_SINGLETON_EXISTS_ERROR, this.GetType().Name);

                // Destroy the entire game object this behaviour is attached to
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}