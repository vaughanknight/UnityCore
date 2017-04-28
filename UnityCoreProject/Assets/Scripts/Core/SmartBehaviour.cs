using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour that wraps all the bits that have heavy
/// performace overhead with common 'getComponent' calls.
/// 
/// Why 'Smart'behaviour?  Because seriously.
/// 
/// NOTE: Feel free to add common accessors, generally which relate
/// to those that have been deprecated for .GetComponent<T> which assumed
/// a single instance i.e transform, renderer, rigidbody etc
/// </summary>
public class SmartBehaviour : MonoBehaviour {

    private Transform _transform;

    public Transform _Transform
    {
        get
        {
            if(_transform == null)
            {
                _transform = this.transform;
            }
            return _transform;
        }
    }

    private Renderer _renderer;

    public Renderer _Renderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = this.GetComponent<Renderer>();
            }
            return _renderer;
        }
    }

    private Rigidbody _rigidBody;

    public Rigidbody _RigidBody
    {
        get
        {
            if (_rigidBody == null)
            {
                _rigidBody = this.GetComponent<Rigidbody>();
            }
            return _rigidBody;
        }
    }
}
