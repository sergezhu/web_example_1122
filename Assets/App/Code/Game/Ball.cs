using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] private float _startForce;
    [SerializeField] private float _startTorque;
    [SerializeField] private Vector2 PushForceRange;
    [SerializeField] private float _targetVelocity;
    [SerializeField] private float _minX;
    [SerializeField] private float _maxX;
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

    public void SetPosition( float arrowPosition )
    {
        var x = Mathf.Lerp( _minX, _maxX, arrowPosition );
        var localPos = transform.localPosition;
        localPos.x = x;
        transform.localPosition = localPos;
    }

    public void Push()
    {
        var randomForceFactor = Random.Range( PushForceRange.x, PushForceRange.y );
        
        _rb.isKinematic = false;
        _rb.AddForce( _startForce * randomForceFactor * Vector3.forward );
        _rb.AddTorque( _startTorque * randomForceFactor * Vector3.right );

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
