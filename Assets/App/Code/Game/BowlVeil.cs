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
		
		public ReactiveCommand Closed { get; } = new ReactiveCommand();
		public ReactiveCommand Opened { get; } = new ReactiveCommand();

		private Tween _tween;

		public void Close()
		{
			_tween?.Kill();
			var t = transform;

			_tween = DOVirtual.Float( 0f, 1f, _openCloseDuration, value =>
				{
					var lerpY = Mathf.Lerp( _openPositionY, _closePositionY, value );
					var pos = t.localPosition;
					pos.y = lerpY;
					t.localPosition = pos;
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
			var t = transform;

			_tween = DOVirtual.Float( 0f, 1f, _openCloseDuration, value =>
				{
					var lerpY = Mathf.Lerp( _closePositionY, _openPositionY, value );
					var pos = t.localPosition;
					pos.y = lerpY;
					t.localPosition = pos;
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