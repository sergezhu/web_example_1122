﻿namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using DG.Tweening;
	using UniRx;
	using UnityEngine;

	public enum RowState
	{
		Stopped,
		Accelerate,
		MaxSpeed,
		Decelerate,
		Finishing
	}
	
	public class Row : MonoBehaviour
	{
		private const int PicsVisible = 3;
		private const int Total = 8;
		
		[SerializeField] private RectTransform _contentRect;
		[SerializeField] private List<Pic> _pics;

		private float _currentRelativeOffset;
		private float _currentRelativeSpeed;
		private bool _isStopped;
		private int _picIndex;
		private float _size;
		private Tween _tween;

		private readonly ReactiveProperty<RowState> _state = new(RowState.Stopped);
		private int _finishTurns;
		private float _speedThreshold;
		private GameSettings _settings;
		private bool _finishPicPassed;

		private void Awake()
		{
			_state.Subscribe( v => Debug.Log( $"{name} : {v}" ) );

			Observable
				.EveryUpdate()
				.DelayFrame( 2 )
				.First()
				.Subscribe( _ => InitRect() )
				.AddTo( this );
		}

		public void Construct(GameSettings settings)
		{
			_settings = settings;
			_speedThreshold = _settings.SpeedThreshold;

			foreach ( var pic in _pics )
			{
				pic.Construct( _speedThreshold );
			}
		}

		private void InitRect()
		{
			_size = _contentRect.sizeDelta.y * Total / _pics.Count;

			Debug.Log( $"size : {_contentRect.rect.height}");
		}

		private void Update()
		{

			if ( _state.Value == RowState.Accelerate || _state.Value == RowState.MaxSpeed ||
				 _state.Value == RowState.Decelerate || _state.Value == RowState.Finishing )
			{
				var delta = _currentRelativeSpeed * Time.deltaTime;
				_currentRelativeOffset += delta;

				if ( _state.Value == RowState.Finishing )
				{
					var overIndex = OffsetToIndex( _contentRect.anchoredPosition.y );

					if ( overIndex == _settings.TargetIndex && !_finishPicPassed )
					{
						if ( _finishTurns > 0 )
							_finishTurns--;
						else
							FullStop();

						_finishPicPassed = true;
					}
					else
					{
						_finishPicPassed = false;
					}
				}
			}

			if ( _state.Value != RowState.Stopped )
			{
				_currentRelativeOffset -= (int) _currentRelativeOffset;

				SetRectPos();
				UpdatePicsViews();

				//Debug.Log( $"offset = {_currentRelativeOffset}, speed = {_currentRelativeSpeed}" );
			}
		}

		[ContextMenu("SetIndex")]
		private void SetIndex()
		{
			var index = _settings.TargetIndex;
			_currentRelativeOffset = (float) index / Total;
			SetRectPos();
		}

		private float IndexToOffset( int index )
		{
			return IndexToRelativeOffset(index) * _size;
		}

		private float IndexToRelativeOffset( int index )
		{
			return (float) index / Total;
		}

		private int OffsetToIndex( float offset )
		{
			return Mathf.FloorToInt( offset / _size * Total );
		}

		private void SetRectPos()
		{
			var currentOffset = _currentRelativeOffset * _size;
			_contentRect.anchoredPosition = new Vector2( _contentRect.anchoredPosition.x, currentOffset );
		}

		public void StartSpin( float duration, float targetMaxSpeed )
		{
			if(_state.Value != RowState.Stopped)
				return;

			_state.Value = RowState.Accelerate;
			TweenSpeed( duration, targetMaxSpeed, () => _state.Value = RowState.MaxSpeed );
		}

		public void Brake( int picIndex, float duration, float targetMinSpeed )
		{
			if ( _state.Value != RowState.Accelerate && _state.Value != RowState.MaxSpeed )
				return;

			_state.Value = RowState.Decelerate;
			TweenSpeed( duration, targetMinSpeed, () => Finishing( picIndex, _settings.LoopsBeforeStop ) );
		}

		private void Finishing( int picIndex, int turns = 1 )
		{
			_state.Value = RowState.Finishing;
			
			//var offset = (float) picIndex / Total;
			var offset = IndexToRelativeOffset( picIndex );
			_finishTurns = _currentRelativeOffset < offset ? turns + 1 : turns;

			Debug.Log( $"Finishing : curOf {_currentRelativeOffset}, curSp {_currentRelativeSpeed}, turns : {_finishTurns}" );
			
			/*var duration = (_finishRelativeOffset - _currentRelativeOffset) / _currentRelativeSpeed;
			
			_tween = DOVirtual
				.Float( _currentRelativeOffset, _finishRelativeOffset, duration, v =>
				{
					_currentRelativeSpeed = (v - _currentRelativeOffset) / Time.deltaTime;
					_currentRelativeOffset = v;
				} )
				.SetEase( Ease.Linear )
				.OnComplete( () =>
				{
					_tween = null;
					_state.Value = RowState.Stopped;
					_currentRelativeSpeed = 0;
					UpdatePicsViews( _currentRelativeSpeed );
				} );*/
		}

		private void FullStop()
		{
			_state.Value = RowState.Stopped;
			_currentRelativeSpeed = 0;
			_currentRelativeOffset = _finishTurns;
			
			SetRectPos();
			UpdatePicsViews();
		}

		private void TweenSpeed( float duration, float newSpeed, Action onComplete = null )
		{
			_tween?.Kill();
			_tween = DOVirtual
				.Float( _currentRelativeSpeed, newSpeed, duration, v => _currentRelativeSpeed = v )
				.SetEase( Ease.InOutCubic )
				.OnComplete( () =>
				{
					_tween = null;
					onComplete?.Invoke();
				} );
		}


		private void UpdatePicsViews()
		{
			//Debug.Log( $"{_currentRelativeSpeed}" );
			_pics.ForEach( pic => pic.UpdateView( _currentRelativeSpeed ) );
		}
	}
}