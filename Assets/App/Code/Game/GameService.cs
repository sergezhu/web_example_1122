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
		private KeglesController _keglesController;


		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand<bool> WordSelected { get; } = new();

		public void Construct( GameView view, GameSettings settings, KeglesController keglesController ) 
		{
			_view = view;
			_settings = settings;
			_keglesController = keglesController;
		} 

		public void Run()
		{
			_view.Show();
			
			/*_view.NextButtonClick
				.Subscribe( _ => OnNextButtonClick())
				.AddTo( this );*/
			
			_view.PushButtonClick
				.Subscribe( _ => OnPushButtonClick())
				.AddTo( this );

			_view.ExitButtonClick
				.Subscribe( _ => Application.Quit() )
				.AddTo( this );

			_keglesController.KeglesFaultCount
				.Subscribe( v => OnKeglesFaultCount( v ) )
				.AddTo( this );

			_view.NearFinishEnter
				.Subscribe( _ => OnNearFinished() )
				.AddTo( this );

			_view.FinishEnter
				.Subscribe( _ => OnFinished() )
				.AddTo( this );

			StartGame();
			GameStarted.Execute();
		}

		public void StartGame()
		{
			_keglesController.Initialize();
			_keglesController.UnlockEventFiring();
			
			_view.ShowArrowsBlock();
			_view.EnablePushButton();
		}

		private void OnPushButtonClick()
		{
			Debug.Log( "PUSH" );
			
			_view.HideArrowsBlock();
			_view.DisablePushButton();

			var pointerPos = _view.ArrowsBlock.CurrentPointerProgress;
			_view.Ball.SetPosition( pointerPos );
			_view.Ball.Push();
		}

		private void OnNextButtonClick()
		{
			CleanUp();
			StartGame();
		}

		private void OnKeglesFaultCount( int value )
		{
			_view.LedView.SetScores( value, _keglesController.KeglesCount );
		}

		private void OnNearFinished()
		{
		}

		private void OnFinished()
		{
			
		}

		private async void FinishAsync()
		{
			var delayTask1 = Task.Delay( TimeSpan.FromSeconds( _settings.FallingKeglesDuration ) );
			await delayTask1;

			// lock kegles
			_keglesController.LockEventFiring();
			
			// handle win or lose
			var isWin = _keglesController.KeglesFaultCount.Value >= _settings.WinKeglesCount;
			var result = isWin ? "WIN" : "LOSE";
			Debug.Log( result );

			var delayTask2= Task.Delay( TimeSpan.FromSeconds( _settings.ResultShowDuration ) );
			await delayTask2;


			// show veil
			// reset kegles
			// start revert ball
			// hide veil
		}

		private void CleanUp()
		{
		}
	}
}