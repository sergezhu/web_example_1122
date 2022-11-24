namespace App.Code.Audio
{
	using System;
	using App.Code.Game;
	using UniRx;

	public class AudioController : IDisposable
	{
		private AudioLibrary _lib;
		private GameService _gameService;

		private CompositeDisposable _disposables;

		public AudioController( AudioLibrary lib, GameService gameService )
		{
			_disposables = new CompositeDisposable();
			_lib = lib;
			_gameService = gameService;

			Subscribe();
		}

		public void Dispose()
		{
			_disposables?.Dispose();
		}

		private void Subscribe()
		{
			_gameService.GameStarted
				.Subscribe( _ => PlayBGMusic() )
				.AddTo( _disposables );

			_gameService.StopWithLose
				.Subscribe( _ => { } )
				.AddTo( _disposables );

			_gameService.StopWithWin
				.Subscribe( _ =>
				{
					PlayRewardMusic();
					PlayRewardCoins();
				} )
				.AddTo( _disposables );

			_gameService.SpinStarted
				.Subscribe( _ => PlaySpin() )
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

		private void PlaySpin()
		{
			_lib.Spin?.Play();
		}

		private void PlayRewardCoins()
		{
			_lib.RewardCoins?.Play();
		}

		private void PlayRewardMusic()
		{
			_lib.RewardMusic?.Play();
		}

		private void PlayButtonClick()
		{
			_lib.ButtonsClick?.Play();
		}
	}
}