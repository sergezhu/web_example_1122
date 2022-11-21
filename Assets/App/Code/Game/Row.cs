namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using DG.Tweening;
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
		[SerializeField] private RectTransform _contentRect;
		[SerializeField] private List<Pic> _pics;

		private float _currentOffset;
		private float _currentSpeed;
		private bool _isStopped;
		private int _picIndex;
		private int _total;
		private float _size;
		private Tween _tween;
		private RowState _state;

		private void Awake()
		{
			_total = _pics.Count / 2; // second half is copy for animation
			_size = _contentRect.sizeDelta.y;
		}

		private void Update()
		{
			if(_state == RowState.Accelerate || _state == RowState.MaxSpeed || _state == RowState.Decelerate)
				_currentOffset += _currentSpeed * Time.deltaTime;

			if ( _state != RowState.Stopped )
			{
				_currentOffset = Mathf.Repeat( _currentOffset, _size );
				_contentRect.anchoredPosition = new Vector2( _contentRect.anchoredPosition.x, -_currentOffset );
				
				UpdatePicsViews( _currentSpeed );
			}
		}

		public void StartSpin( float duration, float targetMaxSpeed )
		{
			if(_state != RowState.Stopped)
				return;

			TweenSpeed( duration, targetMaxSpeed );
		}

		public void StopSpin( int picIndex, float duration, float targetMinSpeed )
		{
			if ( _state != RowState.Accelerate && _state != RowState.MaxSpeed )
				return;

			_state = RowState.Decelerate;

			TweenSpeed( duration, targetMinSpeed, () => Finishing( picIndex ) );
		}

		[ContextMenu("Test Start")]
		private void TestStart()
		{
			StartSpin( 2, 50 );
		}

		[ContextMenu( "Test Stop" )]
		private void TestStop()
		{
			StopSpin( UnityEngine.Random.Range(0, 8), 2, 50 );
		}
		

		private void Finishing( int picIndex )
		{
			_state = RowState.Finishing;
			
			var offset = (float) picIndex / _total * _size;
			var mul = _currentOffset < offset ? 2 : 1;
			var targetOffset = offset + mul * _size;
			var duration = (targetOffset - _currentOffset) / _currentSpeed;

			_tween = DOVirtual
				.Float( _currentOffset, targetOffset, duration, v =>
				{
					_currentSpeed = (v - _currentOffset) / Time.deltaTime;
					_currentOffset = v;
				} )
				.SetEase( Ease.OutQuad )
				.OnComplete( () =>
				{
					_tween = null;
					_state = RowState.Stopped;
					_currentSpeed = 0;
					UpdatePicsViews( _currentSpeed );
				} );
		}

		private void TweenSpeed( float duration, float newSpeed, Action onComplete = null )
		{
			_tween?.Kill();
			_tween = DOVirtual
				.Float( _currentSpeed, newSpeed, duration, v => _currentSpeed = v )
				.SetEase( Ease.InOutCubic )
				.OnComplete( () =>
				{
					_tween = null;
					onComplete?.Invoke();
				} );
		}


		private void UpdatePicsViews(float speed)
		{
			_pics.ForEach( pic => pic.UpdateView( speed ) );
		}
	}
}