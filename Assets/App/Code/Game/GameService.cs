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
		private string[] _sportWords;
		private string[] _nonSportWords;
		private string[] _currentWords;
		private int _currentRightIndex;

		
		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand<bool> WordSelected { get; } = new();

		public void Construct(GameView view, GameSettings settings) 
		{
			_view = view;
			_settings = settings;
		} 

		public void Run()
		{
			_view.Show();
			
			_view.NextButtonClick
				.Subscribe( _ => OnNextButtonClick())
				.AddTo( this );

			_view.ExitButtonClick
				.Subscribe( _ => Application.Quit() )
				.AddTo( this );

			StartGame();
			GameStarted.Execute();
		}

		public void StartGame()
		{
		}

		private void OnNextButtonClick()
		{
			CleanUp();
			StartGame();
		}

		private void CleanUp()
		{
		}
	}
}