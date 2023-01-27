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

			StartGame();
			GameStarted.Execute();
		}

		public void StartGame()
		{
			_keglesController.Initialize();
			_view.ShowArrowsBlock();
		}

		private void OnPushButtonClick()
		{
			Debug.Log( "PUSH" );
			
			_view.HideArrowsBlock();
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

		private void CleanUp()
		{
		}
	}
}