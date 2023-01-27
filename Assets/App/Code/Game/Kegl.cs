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
    [SerializeField] private Vector3 _defaultPosition;
    [SerializeField] private Quaternion _defaultRotation;
    
    private int _groundLayerMask;
    private Transform _transform;
    private WaitForSeconds _groundCheckWaiter;
    private GameSettings _settings;
    private Coroutine _routine;

    public ReactiveProperty<bool> IsFault { get; } = new ReactiveProperty<bool>();

    public void Construct(GameSettings settings)
    {
        _transform = transform;
        _settings = settings;
        
        _groundLayerMask = LayerMask.GetMask( "Ground" );
        _defaultPosition = _rb.position;
        _defaultRotation = _rb.rotation;

        if ( Physics.Raycast( _groundDetectPoint.position, Vector3.down, out var hitInfo, _groundLayerMask ) )
            _defaultGroundDistance = hitInfo.distance;
        else
            Debug.LogWarning( $"Ground not found" );

        _groundCheckWaiter = new WaitForSeconds(0.1f);
    }

    public void Initialize()
    {
        IsFault.Value = false;
        
        _rb.position = _defaultPosition;
        _rb.rotation = _defaultRotation;
        _rb.velocity = Vector3.zero;
        
        StartGroundCheck();
    }

    private void StartGroundCheck()
    {
        if ( _routine != null )
            return;
        
        _routine = StartCoroutine( GroundCheckRoutine() );
    }

    private void StopGroundCheck()
    {
        if ( _routine == null )
            return;

        StopCoroutine( _routine );
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
        if ( Physics.Raycast( _groundDetectPoint.position, Vector3.down, out var hitInfo, _groundLayerMask ) )
        {
            if ( hitInfo.distance <= _settings.KegleFallDistance )
            {
                IsFault.Value = true;
                StopGroundCheck();
            }
        }
    }
}
