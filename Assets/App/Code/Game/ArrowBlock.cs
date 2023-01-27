namespace App.Code.Game
{
	using DG.Tweening;
	using UnityEngine;

	public class ArrowBlock : MonoBehaviour
	{
		[SerializeField] private Transform _leftPoint;
		[SerializeField] private Transform _rightPoint;
		[SerializeField] private Transform _pointerTransform;
		[SerializeField] private CanvasGroup _arrowBlockGroup;

		[Space]
		[SerializeField] private float _fadeDuration = 0.5f;
		[SerializeField] private float _pointerDuration = 2f;

		private Tween _fadeAnimation;
		private Tween _pointerAnimation;
		private float _currentPointerProgress;

		public void Show()
		{
			StartPointerAnimation();

			_fadeAnimation?.Kill();
			_fadeAnimation = _arrowBlockGroup
				.DOFade( 1, _fadeDuration )
				.OnComplete( () => { _fadeAnimation = null; } );
		}

		public void Hide()
		{
			_fadeAnimation?.Kill();
			_fadeAnimation = _arrowBlockGroup
				.DOFade( 0, _fadeDuration )
				.OnComplete( () =>
				{
					_fadeAnimation = null;
					StopPointerAnimation();
				} );
		}
		
		private void StartPointerAnimation()
		{
			_pointerAnimation = DOVirtual.Float( 0, 1, _pointerDuration, value =>
				{
					_currentPointerProgress = value;
					_pointerTransform.position = Vector3.Lerp( _leftPoint.position, _rightPoint.position, value );
				} )
				.SetEase( Ease.OutCubic )
				.SetLoops( -1, LoopType.Yoyo );
		}

		private void StopPointerAnimation()
		{
			_pointerAnimation?.Kill();
		}
	}
}