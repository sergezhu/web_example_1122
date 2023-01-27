using System;
using DG.Tweening;
using UniRx;
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

    [Header("Revert Settings")]
    [SerializeField] private Transform _revertStartPointL;
    [SerializeField] private Transform _revertFinishPointL;
    [SerializeField] private Transform _revertStartPointR;
    [SerializeField] private Transform _revertFinishPointR;
    [SerializeField] private float _revertDuration = 5f;
    
    private bool _active;
    private Vector3 _defaultPosition;
    private Tween _revertTween;
    private Transform _transform;

    public ReactiveCommand BallReverted { get; } = new ReactiveCommand();

    public void Construct()
    {
        _transform = transform;
        _defaultPosition = _transform.localPosition;
    }

    public void Initialize()
    {
        _rb.isKinematic = true;
        _transform.localPosition = _defaultPosition;
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

    public void Revert()
    {
        _rb.isKinematic = true;
        
        var isLeft = transform.localPosition.x < 0;
        var revertStartPoint = isLeft ? _revertStartPointL : _revertStartPointR;
        var revertFinishPoint = isLeft ? _revertFinishPointL : _revertFinishPointR;
        var dist = (revertFinishPoint.position - revertStartPoint.position).magnitude;

        var size = transform.lossyScale.x;
        var angleMaxRad = -2f * dist / size;

        transform.position = revertStartPoint.position;
        transform.rotation = Quaternion.identity;

        _revertTween?.Kill();
        _revertTween = DOVirtual.Float( 0, 1f, _revertDuration, value =>
            {
                transform.position = Vector3.Lerp( revertStartPoint.position, revertFinishPoint.position, value );
                transform.rotation = Quaternion.Euler( value * angleMaxRad * Mathf.Rad2Deg, 0, 0 );
            } )
            .SetEase( Ease.Linear )
            .OnComplete( () =>
            {
                _revertTween = null;
                BallReverted.Execute();
            } );
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
