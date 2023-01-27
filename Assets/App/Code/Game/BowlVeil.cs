namespace App.Code.Game
{
	using DG.Tweening;
	using UniRx;
	using UnityEngine;

	public class BowlVeil : MonoBehaviour
	{
		[SerializeField] private float _openPositionY;
		[SerializeField] private float _closePositionY;
		[SerializeField] private float _openCloseDuration = 1f;

		private Tween _tween;
		private Transform _transform;

		private Transform Transform => _transform ??= transform;

		public ReactiveCommand Closed { get; } = new ReactiveCommand();
		public ReactiveCommand Opened { get; } = new ReactiveCommand();
		

		public void Close()
		{
			_tween?.Kill();

			_tween = DOVirtual.Float( 0f, 1f, _openCloseDuration, value =>
				{
					var lerpY = Mathf.Lerp( _openPositionY, _closePositionY, value );
					var pos = Transform.localPosition;
					pos.y = lerpY;
					Transform.localPosition = pos;
				} )
				.SetEase( Ease.InOutQuad )
				.OnComplete( () =>
				{
					_tween = null;
					Closed.Execute();
				} );
		}

		public void Open()
		{
			_tween?.Kill();

			_tween = DOVirtual.Float( 0f, 1f, _openCloseDuration, value =>
				{
					var lerpY = Mathf.Lerp( _closePositionY, _openPositionY, value );
					var pos = Transform.localPosition;
					pos.y = lerpY;
					Transform.localPosition = pos;
				} )
				.SetEase( Ease.InOutQuad )
				.OnComplete( () =>
				{
					_tween = null;
					Opened.Execute();
				} );
		}
	}
}