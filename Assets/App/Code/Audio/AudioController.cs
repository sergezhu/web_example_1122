namespace App.Code.Audio
{
	using System;
	using System.Threading.Tasks;
	using App.Code.Game;
	using UniRx;

	public class AudioController : IDisposable
	{
		private AudioLibrary _lib;
		private GameService _gameService;
		private readonly GameView _gameView;

		private CompositeDisposable _disposables;
		private bool _tickerEnabled;
		private int _ticToc;

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

			_gameService.SecondsMeterStarted
				.Subscribe( _ => StartTicker() )
				.AddTo( _disposables ); 

			_gameService.SecondsMeterStopped
				.Subscribe( _ => StopTicker() )
				.AddTo( _disposables );

			_gameService.MatchStarted
				.Subscribe( _ => PlayStart() )
				.AddTo( _disposables );
		}

		private void OnBallHit( (float, float) tuple )
		{
			PlayHit( tuple.Item1, tuple.Item2 );
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

		private void OnResultReady( int v )
		{
			switch ( v )
			{
				case 1:
					PlayWin();
					break;
				
				case -1:
					PlayLose();
					break;
			}
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

		private void PlayHit(float volume, float tone)
		{
			_lib.Hit.volume = volume;
			_lib.Hit.pitch = tone;
			_lib.Hit.Play();
		}

		private void PlayTick( bool even )
		{
			var src = even ? _lib.Tic : _lib.Toc;
			src.Play();
		}

		private async void StartTicker()
		{
			if ( _tickerEnabled )
				return;

			_tickerEnabled = true;
			_ticToc = 0;

			while ( _tickerEnabled )
			{
				PlayTick( _ticToc % 2 == 0 );
				_ticToc++;
				
				await Task.Delay( TimeSpan.FromSeconds( 0.5f ) );
			}
		}

		private void StopTicker()
		{
			_tickerEnabled = false; 
		}
	}
}