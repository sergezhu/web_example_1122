namespace App.Code.Game
{
	using System;
	using System.Threading.Tasks;
	using UniRx;
	using UnityEngine;

	public class GameService : MonoBehaviour
	{
		private GameView _view;
		private GameSettings _settings;
		private int _comboOffset;
		private bool _isWin;
		private int _currentRightIndex;
		private bool _finishedFlag;
		private bool _nearFinishedFlag;


		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand<bool> ResultReady { get; } = new();

		public void Construct( GameView view, GameSettings settings ) 
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
			_finishedFlag = false;
			_nearFinishedFlag = false;
			
			_view.EnableNextButton();
		}

		private void OnNextButtonClick()
		{
			_view.DisableNextButton();

			CleanUp();
			StartGame();
		}


		private void OnNearFinished()
		{
		}

		private void CleanUp()
		{
		}
	}
}