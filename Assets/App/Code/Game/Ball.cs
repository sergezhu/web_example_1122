using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _startForce;
    [SerializeField] private float _targetVelocity;
    [SerializeField] private Rigidbody _rb;
    
    private bool _active;

    [ContextMenu("Push")]
    void Push()
    {
        //_rb.AddForce( _startForce * Vector3.forward );

        _active = true;
    }

    private void FixedUpdate()
    {
        if(!_active)
            return;

        var rbVelocity = _rb.velocity;
        rbVelocity.z = _targetVelocity;
        _rb.velocity = rbVelocity;
    }
}
