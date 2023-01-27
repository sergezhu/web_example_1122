namespace App.Code.Audio
{
	using System;
	using App.Code.Game;
	using UniRx;

	public class AudioController : IDisposable
	{
		private AudioLibrary _lib;
		private GameService _gameService;
		private readonly GameView _gameView;

		private CompositeDisposable _disposables;

		public AudioController( AudioLibrary lib, GameService gameService, GameView gameView )
		{
			_disposables = new CompositeDisposable();
			_lib = lib;
			_gameService = gameService;
			_gameView = gameView;

			Subscribe();
		}

		public void Dispose()
		{
			_disposables?.Dispose();
		}

		private void Subscribe()
		{
			_gameService.ResultReady
				.Subscribe( v => OnResultReady(v) )
				.AddTo( _disposables );

			_gameView.AnyButtonClick
				.Subscribe( _ => PlayClick() )
				.AddTo( _disposables );
		}

		private void PlayBGMusic()
		{
			if ( !_lib.BgMusic.isPlaying )
				_lib.BgMusic.Play();
		}

		private void StopBGMusic()
		{
			if ( _lib.BgMusic.isPlaying )
				_lib.BgMusic.Stop();
		}

		private void OnResultReady( bool v )
		{
			if(v)
				PlayWin();
			else
				PlayLose();
		}

		private void PlayStart()
		{
			_lib.Start.Play();
		}

		private void PlayWin()
		{
			_lib.WinResult.Play();
		}

		private void PlayLose()
		{
			_lib.LoseResult.Play();
		}

		private void PlayClick()
		{
			_lib.Click.Play();
		}
	}
}