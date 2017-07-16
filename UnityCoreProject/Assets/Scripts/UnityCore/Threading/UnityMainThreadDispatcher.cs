/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;

namespace UnityCore.Threading
{
    public class UnityMainThreadDispatcher
    {
        private static IUnityMainThreadDispatcher _instance;
        
        public static void Initialise()
        {
            if (_instance == null)
            {
                _instance = Current();
            }
        }

        public static void ForceInitialise()
        {
            _instance = Current();
        }
        public static IUnityMainThreadDispatcher Current()
        {
            IUnityMainThreadDispatcher dispatcher = null;
            try
            {
                // NOTE: This causes strange behaviour when you do async from the editor while the 
                // game is playing.  What happens is the editor async calls end up hooking
                // the main game loop.  Any ideas, let me know how you can find out if you are on
                // the Game thread, or the Editor thread.

#if UNITY_EDITOR
                // There is an issue here, that being if code is being ran
                // in the editor UI and in the player, you can end up with editor UI
                // code running on the player thread.  It's backwards.
                // Not sure how to avoid this for now.
                if (!Application.isEditor || Application.isPlaying)
                {
                    dispatcher = UnityGameThreadDispatcher.Instance;
                }
                else
                {
                    dispatcher = UnityEditorThreadDispatcher.Instance;
                }
#else
                dispatcher = UnityGameThreadDispatcher.Instance;
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("UnityMainThreadDispatcher.Initialise() must be called from the main thread prior to dispatching back to the main thread.");
                Debug.LogError(e.StackTrace);
            }
            return dispatcher;
        }

        public static IUnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    Initialise();
                }
                return _instance;
            }
        }
    }
}