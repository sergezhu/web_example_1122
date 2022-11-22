namespace App.Code.Game
{
	using UniRx;
	using UnityEngine;

	public class GameService
	{
		private readonly GameView _view;
		private readonly GameSettings _settings;
		private readonly CompositeDisposable _disposables;

		private bool _toggle;

		public GameService(GameView view, GameSettings settings)
		{
			_view = view;
			_settings = settings;
			_disposables = new CompositeDisposable();
			
			foreach ( var row in _view.Rows )
			{
				row.Construct( _settings );
			}
		}

		public void Start()
		{
			_view.SpinButtonClick
				.Subscribe( _ => OnSpinButtonClick() )
				.AddTo( _disposables );
		}

		private void OnSpinButtonClick()
		{
			_toggle = !_toggle;
			
			if(_toggle)
				StartSpin();
			else
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
			foreach ( var row in _view.Rows )
			{
				row.Brake( _settings.TargetIndex, _settings.DecelerateDuration, _settings.MinSpeed );
			}
		}
	}
}