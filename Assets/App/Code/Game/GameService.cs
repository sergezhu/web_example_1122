namespace App.Code.Game
{
	using UniRx;
	using UnityEngine;

	public class GameService : MonoBehaviour
	{
		private GameView _view;
		private GameSettings _settings;
		private int _comboOffset;
		private bool _isWin;

		public ReactiveCommand StopWithWin { get; } = new();
		public ReactiveCommand StopWithLose { get; } = new();
		public ReactiveCommand SpinStarted { get; } = new();
		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand AnyRowTurnPassed { get; } = new();

		public void Construct(GameView view, GameSettings settings) 
		{
			_view = view;
			_settings = settings;
		} 

		public void Run()
		{
			_view.Show();
			
			_view.SpinButtonClick
				.Subscribe( _ => { } )
				.AddTo( this );

			_view.ExitButtonClick
				.Subscribe( _ => Application.Quit() )
				.AddTo( this );

			GameStarted.Execute();
		}
	}
}