using System;
using System.Collections;
using App.Code.Game;
using UniRx;
using UnityEngine;

public class Kegl : MonoBehaviour
{
    [SerializeField] private Transform _centerOfMass;
    [SerializeField] private Transform _groundDetectPoint;
    [SerializeField] private Rigidbody _rb;

    [Header( "Debug" )]
    [SerializeField] private float _defaultGroundDistance;
    [SerializeField] private float _fallGroundDistance;
    [SerializeField] private float _currentGroundDistance;
    [SerializeField] private Vector3 _defaultPosition;
    [SerializeField] private Quaternion _defaultRotation;
    [SerializeField] private bool _routineActive;
    
    private int _groundLayerMask;
    private Transform _transform;
    private WaitForSeconds _groundCheckWaiter;
    private GameSettings _settings;
    private Coroutine _routine;
    private int _maxDistance;

    public ReactiveProperty<bool> IsFault { get; } = new ReactiveProperty<bool>();

    public void Construct(GameSettings settings)
    {
        _transform = transform;
        _settings = settings;
        
        _groundLayerMask = LayerMask.GetMask( "Boundary" );
        _maxDistance = 5;
        _defaultPosition = _rb.position;
        _defaultRotation = _rb.rotation;

        if ( Physics.Raycast( _groundDetectPoint.position, Vector3.down, out var hitInfo, _maxDistance, _groundLayerMask ) )
        {
            _defaultGroundDistance = _groundDetectPoint.position.y - hitInfo.point.y;
            _fallGroundDistance = _defaultGroundDistance * _settings.KegleFallDistanceFactor;
        }
        else
            Debug.LogWarning( $"Ground not found" );

        _groundCheckWaiter = new WaitForSeconds(0.1f);
    }

    public void Initialize()
    {
        IsFault.Value = false;
        StartGroundCheck();
    }

    public void ResetPosition()
    {
        _rb.position = _defaultPosition;
        _rb.rotation = _defaultRotation;
        _rb.velocity = Vector3.zero;
    }

    public void CleanUp()
    {
        StopGroundCheck();
    }

    private void StartGroundCheck()
    {
        if ( _routine != null )
            return;
        
        _routine = StartCoroutine( GroundCheckRoutine() );
        _routineActive = true;
    }

    private void StopGroundCheck()
    {
        if ( _routine == null )
            return;

        StopCoroutine( _routine );
        _routine = null;
        _routineActive = false;
    }

    private IEnumerator GroundCheckRoutine()
    {
        while ( true )
        {
            GroundCheck();
            yield return _groundCheckWaiter;
        }
    }

    private void GroundCheck()
    {
        if ( Physics.Raycast( _groundDetectPoint.position, Vector3.down, out var hitInfo, _maxDistance, _groundLayerMask ) )
        {
            _currentGroundDistance = _groundDetectPoint.position.y - hitInfo.point.y;
            
            Debug.DrawLine( _groundDetectPoint.position, _groundDetectPoint.position + _currentGroundDistance * Vector3.down, Color.red, 1f );
            
            if ( _currentGroundDistance <= _fallGroundDistance )
            {
                IsFault.Value = true;
                StopGroundCheck();
            }
        }
    }
}
