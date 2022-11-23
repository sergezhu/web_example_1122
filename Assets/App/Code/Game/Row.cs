namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using DG.Tweening;
	using UniRx;
	using UnityEngine;
	using Random = UnityEngine.Random;

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
		public const int Total = 7;
		
		[SerializeField] private RectTransform _contentRect;
		[SerializeField] private List<Pic> _pics;

		private float _currentRelativeOffset;
		private float _currentRelativeSpeed;
		private bool _isStopped;
		private int _picIndex;
		private float _size;
		private Tween _tween;
		private GameSettings _settings;

		private readonly ReactiveProperty<RowState> _state = new(RowState.Stopped);
		public IReadOnlyList<Pic> Pics => _pics;

		private int _rowIndex;
		private int _finishTurns;
		private float _speedThreshold;
		private bool _finishPicPassed;
		private int _targetIndex;
		public bool CanSpin { get; private set; }


		private void Awake()
		{
			_state.Subscribe( v =>
			{
				CanSpin = v == RowState.Stopped;
				Debug.Log( $"{name} : {v}" );
			} );

			Observable
				.EveryUpdate()
				.DelayFrame( 2 )
				.First()
				.Subscribe( _ => InitRect() )
				.AddTo( this );
		}

		public void Construct(int id, GameSettings settings)
		{
			_rowIndex = id;
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

					if ( overIndex == _targetIndex && !_finishPicPassed )
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

		public void SetForceIndex(int index)
		{
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
			TweenSpeed( duration, targetMaxSpeed, _settings.RandomDelayBeforeStart, () => _state.Value = RowState.MaxSpeed );
		}

		public void Brake( int picIndex, float duration, float targetMinSpeed )
		{
			if ( _state.Value != RowState.Accelerate && _state.Value != RowState.MaxSpeed )
				return;

			_state.Value = RowState.Decelerate;
			TweenSpeed( duration, targetMinSpeed, 0, () => Finishing( picIndex, _settings.LoopsBeforeStop ) );
		}

		private void Finishing( int picIndex, int turns = 1 )
		{
			_state.Value = RowState.Finishing;
			_targetIndex = picIndex;
			
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
			//_currentRelativeOffset = IndexToOffset( _targetIndex );
			
			SetRectPos();
			UpdatePicsViews();
		}

		private void TweenSpeed( float duration, float newSpeed, float delayBefore, Action onComplete = null )
		{
			var rndDelay = delayBefore <= 0.01f ? 0 : Random.Range( 0.01f, delayBefore );
			
			_tween?.Kill();
			_tween = DOVirtual
				.Float( _currentRelativeSpeed, newSpeed, duration, v => _currentRelativeSpeed = v )
				.SetEase( Ease.InOutCubic )
				.SetDelay( rndDelay )
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