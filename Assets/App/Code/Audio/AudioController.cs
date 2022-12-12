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
			_gameService.WordSelected
				.Subscribe( v => OnWordSelected(v) )
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

		private void OnWordSelected( bool v )
		{
			if(v)
				PlayRightAnswer();
			else
				PlayFailAnswer();
		}

		private void PlayStart()
		{
			_lib.Start.Play();
		}

		private void PlayRightAnswer()
		{
			_lib.RightAnswer.Play();
		}

		private void PlayFailAnswer()
		{
			_lib.FailAnswer.Play();
		}

		private void PlayClick()
		{
			_lib.Click.Play();
		}
	}
}