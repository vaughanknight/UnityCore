using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityCore.Threading;
using UnityEditor;
using UnityEngine;

namespace UnityCore.Demos
{
    public class ThreadTestBehaviour : MonoBehaviour {
        private IWindsorContainer _container;
        private ITester _tester;
        // Use this for initialization
        void Start()
        {
            _container = new WindsorContainer().Install(new ContainerInstaller()); 

            _tester = _container.Resolve<ITester>();

            _tester.Test();
        }

        // Update is called once per frame
        void Update() {

        }
    }
}