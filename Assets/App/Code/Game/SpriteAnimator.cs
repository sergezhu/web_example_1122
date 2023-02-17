namespace App.Code.Game
{
	using System;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;

	public class SpriteAnimator : MonoBehaviour
	{
		[SerializeField] private Image _targetImage;
		[SerializeField] private float _frequency;
		[SerializeField] private Sprite[] _sprites;

		private WaitForSeconds _waiter;
		private int _currentIndex;
		private Coroutine _routine;
		
		private void Awake()
		{
			_waiter = new WaitForSeconds( 1f / _frequency );
		}

		private void OnEnable()
		{
			_currentIndex = 0;
			
			TryStopRoutine();

			_routine = StartCoroutine( Animate() );
		}

		private void OnDisable()
		{
			TryStopRoutine();
		}

		private void TryStopRoutine()
		{
			if ( _routine != null )
				StopCoroutine( _routine );
		}

		private IEnumerator Animate()
		{
			while ( true )
			{
				_targetImage.sprite = _sprites[_currentIndex];
				yield return _waiter;

				_currentIndex = (_currentIndex + 1) % _sprites.Length;

				Debug.Log( $"i : {_currentIndex}" );
			}
		}
	}
}