namespace App.Code.Game
{
	using System.Collections;
	using System.Linq;
	using UniRx;
	using UnityEngine;

	public class GameService : MonoBehaviour
	{
		private GameView _view;
		private GameSettings _settings;

		public void Construct(GameView view, GameSettings settings) 
		{
			_view = view;
			_settings = settings;

			for ( var i = 0; i < _view.Rows.Count; i++ )
			{
				var row = _view.Rows[i];
				row.Construct( i, _settings );
			}
		}

		private bool CanSpin => _view.Rows.All( row => row.CanSpin );

		public void Run()
		{
			_view.SpinButtonClick
				.Subscribe( _ => OnSpinButtonClick() )
				.AddTo( this );
		}

		private void OnSpinButtonClick()
		{
			if ( CanSpin )
			{
				StartCoroutine( SpinAsync() );
			}
			else
			{
				Debug.LogWarning( $"You can not spin now" );
			}
		}

		private IEnumerator SpinAsync()
		{
			var delay = Mathf.Max( _settings.AccelerateDuration + 0.1f, _settings.DelayBeforeStop );
			Debug.LogWarning( $"SPIN! delay : {delay}" );

			StartSpin();

			yield return new WaitForSeconds( delay );
			StopSpin();
		}

		private void StartSpin()
		{
			foreach ( var row in _view.Rows )
			{
				row.StartSpin( _settings.AccelerateDuration, _settings.MaxSpeed );
			}
		}

		private void StopSpin()
		{
			for ( var i = 0; i < _view.Rows.Count; i++ )
			{
				var row = _view.Rows[i];
				row.Brake( _settings.TargetIndex[i], _settings.DecelerateDuration, _settings.MinSpeed );
			}
		}
	}
}