namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using App.Code.Utils;
	using UniRx;
	using UnityEditor.VersionControl;
	using UnityEngine;
	using Random = UnityEngine.Random;
	using Task = System.Threading.Tasks.Task;

	public struct TimeZone
	{
		private readonly Vector2 _timeRange;
		private readonly int _index;
		
		private bool NeedGoal;
		public bool HasGoal;

		public TimeZone( Vector2 timeRange, int index ) : this()
		{
			_timeRange = timeRange;
			_index = index;
		}

		public int Index => _index;
		public int PlayerIndex => _index % 2;
		public Vector2 TimeRange => _timeRange;
	}

	public class GameService : MonoBehaviour
	{
		private GameView _view;
		private GameSettings _settings;
		private int _comboOffset;
		private bool _isWin;
		private int _currentRightIndex;
		private float _currentTime;
		private Task _delayTask;
		private float _timerTick;
		private TimeZone[] _timeZones;


		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand<bool> ResultReady { get; } = new();

		public void Construct( GameView view, GameSettings settings ) 
		{
			_view = view;
			_settings = settings;

			_timerTick = 0.1f;
			_delayTask = Task.Delay( TimeSpan.FromSeconds( _timerTick ) );
		} 

		public void Run()
		{
			_view.Show();

			_view.StartButtonClick
				.Subscribe( _ => OnStartButtonClick() )
				.AddTo( this );
			
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
			_view.SwitchToBetSView();
			RandomizeFlags();
		}

		private void RandomizeFlags()
		{
			_view.FlagLeftRandomizer.Shuffle();
			_view.FlagRightRandomizer.Shuffle(new []{ _view.FlagLeftRandomizer.SpriteIndex});
		}

		private void OnStartButtonClick()
		{
			_view.SwitchToMatchView();

			var leftScore = Random.Range( 0, Mathf.RoundToInt( 0.75f * _settings.MatchMaxScore ) );
			var rightScore = Random.Range( 0, Mathf.RoundToInt( 0.75f * _settings.MatchMaxScore ) );
			
			StartMatch(leftScore, rightScore);
		}

		private void OnNextButtonClick()
		{
			_view.DisableNextButton();

			CleanUp();
			StartGame();
		}


		private void StartMatch(int leftFinalScore, int rightFinalScore)
		{
			SetupTimeZones(leftFinalScore, rightFinalScore);
		}

		private async void StartMatchAsync()
		{
			_currentTime = 0f;
			var matchDuration = _settings.MatchDuration;
			_view.MatchView.UpdateMatchTime( 0, matchDuration );

			while ( _currentTime <= _settings.MatchDuration )
			{
				await _delayTask;
				_currentTime += _timerTick;
				_view.MatchView.UpdateMatchTime( _currentTime, matchDuration );

				TryChangeScores();
			}
		}

		private void SetupTimeZones( int leftFinalScore, int rightFinalScore )
		{
			_timeZones = new TimeZone[_settings.MatchMaxScore * 2];

			var zoneDuration = _settings.MatchDuration / (_settings.MatchMaxScore * 2 + 1);

			for ( var i = 0; i < _timeZones.Length; i++ )
			{
				var t1 = i * zoneDuration;
				var t2 = (i + 1) * zoneDuration;

				_timeZones[i] = new TimeZone( new Vector2( t1, t2 ), i );
			}

			var timeZonesIndexesForLeft = _timeZones.Where( zone => zone.PlayerIndex == 0 ).Select( zone => zone.Index ).ToList();
			var timeZonesIndexesForRight = _timeZones.Where( zone => zone.PlayerIndex == 1 ).Select( zone => zone.Index ).ToList();

			var goalIndexesLeft = new List<int>();
			var goalIndexesRight = new List<int>();

			/*for ( var j = 0; j <= _timeZones.Length; j++ )
			{
				int goalIndex;
				var zone = _timeZones[j];

				if ( zone.PlayerIndex == 0 )
				{
					while ( goalIndexesLeft.Contains( goalIndex = Random.Range( 0, timeZonesIndexesForLeft.Count ) ) )
						continue;
				}
				else
				{
					while ( goalIndexesRight.Contains( goalIndex = Random.Range( 0, goalIndexesRight.Count ) ) )
						continue;
				}
			}*/
			
			for ( var j1 = 0; j1 <= leftFinalScore; j1++ )
			{
				int goalIndex;
				
				while( goalIndexesLeft.Contains( goalIndex = timeZonesIndexesForLeft.Random() ))
					continue;
				
				goalIndexesLeft.Add( goalIndex );
			}

			for ( var j2 = 0; j2 <= rightFinalScore; j2++ )
			{
				int goalIndex;

				while ( goalIndexesRight.Contains( goalIndex = timeZonesIndexesForRight.Random() ) )
					continue;

				goalIndexesRight.Add( goalIndex );
			}

			var leftInd = "Left Indexes";
			goalIndexesLeft.ForEach( s => leftInd = $"{leftInd} {s}" );
			var rightInd = "Right Indexes";
			goalIndexesRight.ForEach( s => rightInd = $"{rightInd} {s}" );
			
			Debug.Log( leftInd );
			Debug.Log( rightInd );
		}

		private void TryChangeScores()
		{
		}

		private void CleanUp()
		{
		}
	}
}