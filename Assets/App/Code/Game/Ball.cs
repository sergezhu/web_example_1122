using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _startForce;
    [SerializeField] private float _startTorque;
    [SerializeField] private float _targetVelocity;
    [SerializeField] private Rigidbody _rb;
    
    private bool _active;
    private Vector3 _defaultPosition;

    public void Construct()
    {
        _defaultPosition = _rb.position;
    }

    public void Initialize()
    {
        _rb.isKinematic = true;
        _rb.position = _defaultPosition;
    }

    [ContextMenu("Push")]
    public void Push()
    {
        _rb.isKinematic = false;
        _rb.AddForce( _startForce * Vector3.forward );
        _rb.AddTorque( _startTorque * Vector3.right );

        _active = true;
    }

    private void FixedUpdate()
    {
        /*if(!_active)
            return;

        var rbVelocity = _rb.velocity;
        rbVelocity.z = _targetVelocity;
        _rb.velocity = rbVelocity;*/
    }
}
