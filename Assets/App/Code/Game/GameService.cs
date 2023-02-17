namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using App.Code.Game.Enums;
	using App.Code.Utils;
	using UniRx;
	using UnityEngine;
	using Random = UnityEngine.Random;
	using Task = System.Threading.Tasks.Task;

	[Serializable]
	public struct TimeZone
	{
		[SerializeField] private Vector2 _timeRange;
		[SerializeField] private int _index;
		
		public bool NeedGoal;
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
		private float _timerTick;
		private List<TimeZone> _timeZones;
		private int _leftScores;
		private int _rightScores;


		public ReactiveCommand GameStarted { get; } = new();
		public ReactiveCommand<bool> ResultReady { get; } = new();

		public bool IsWin => _view.BetView.BetCommand == ECommand.Left && _leftScores > _rightScores ||
							 _view.BetView.BetCommand == ECommand.Right && _leftScores < _rightScores;

		public bool IsLose => _view.BetView.BetCommand == ECommand.Left && _leftScores < _rightScores ||
							 _view.BetView.BetCommand == ECommand.Right && _leftScores > _rightScores;

		public void Construct( GameView view, GameSettings settings ) 
		{
			_view = view;
			_settings = settings;

			_timerTick = 0.1f;
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
			Debug.Log( "Next" );
			
			_view.DisableNextButton();

			CleanUp();
			StartGame();
		}


		private void StartMatch(int leftFinalScore, int rightFinalScore)
		{
			SetupTimeZones(leftFinalScore, rightFinalScore);
			
			StartMatchAsync();
		}

		private async void StartMatchAsync()
		{
			_currentTime = 0f;
			_leftScores = 0;
			_rightScores = 0;
			
			var matchDuration = _settings.MatchDuration;
			_view.MatchView.UpdateMatchTime( 0, matchDuration );
			_view.MatchView.SetLeftScore( 0 );
			_view.MatchView.SetRightScore( 0 );

			await Task.Delay( TimeSpan.FromSeconds( _settings.DelayBeforeMatch ) );

			while ( _currentTime <= _settings.MatchDuration )
			{
				await Task.Delay( TimeSpan.FromSeconds( _timerTick ) );
				_currentTime += _timerTick;
				_view.MatchView.UpdateMatchTime( _currentTime, matchDuration );

				TryChangeScores();
			}

			ShowResult();
		}

		private void SetupTimeZones( int leftFinalScore, int rightFinalScore )
		{
			var count = _settings.MatchMaxScore * 2;
			_timeZones = new List<TimeZone>( count );

			var zoneDuration = _settings.MatchDuration / (count + 1);

			for ( var i = 0; i < count; i++ )
			{
				var t1 = i * zoneDuration;
				var t2 = (i + 1) * zoneDuration;

				_timeZones.Add( new TimeZone( new Vector2( t1, t2 ), i ) );
			}

			var timeZonesIndexesForLeft = _timeZones.Where( zone => zone.PlayerIndex == 0 ).Select( zone => zone.Index ).ToList();
			var timeZonesIndexesForRight = _timeZones.Where( zone => zone.PlayerIndex == 1 ).Select( zone => zone.Index ).ToList();
			
			Debug.Log( $"{_timeZones.Count} : {timeZonesIndexesForLeft.Count} + {timeZonesIndexesForRight.Count}" );

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

				SetNeedGoal( goalIndex );
			}

			for ( var j2 = 0; j2 <= rightFinalScore; j2++ )
			{
				int goalIndex;

				while ( goalIndexesRight.Contains( goalIndex = timeZonesIndexesForRight.Random() ) )
					continue;

				goalIndexesRight.Add( goalIndex );

				SetNeedGoal( goalIndex );
			}

			void SetNeedGoal( int gi )
			{
				var zone = _timeZones[gi];
				zone.NeedGoal = true;
				_timeZones[gi] = zone;
			}

			{	// DEBUG
				_view.SetDebugZones( _timeZones );

				var leftInd = "Left Indexes";
				goalIndexesLeft.ForEach( s => leftInd = $"{leftInd} {s}" );
				var rightInd = "Right Indexes";
				goalIndexesRight.ForEach( s => rightInd = $"{rightInd} {s}" );

				Debug.Log( leftInd );
				Debug.Log( rightInd );
			}
		}

		private void TryChangeScores()
		{
			var currentTimeZoneIndex = _timeZones.FindIndex( zone => _currentTime >= zone.TimeRange.x && _currentTime <= zone.TimeRange.y );
			if ( currentTimeZoneIndex == -1 )
				return;
			
			var currentTimeZone = _timeZones[currentTimeZoneIndex];
			
			if(!currentTimeZone.NeedGoal)
				return;
			
			if(currentTimeZone.HasGoal)
				return;

			currentTimeZone.HasGoal = true;
			_timeZones[currentTimeZoneIndex] = currentTimeZone;
			
			HandleGoal( currentTimeZone );
		}

		private void HandleGoal( TimeZone currentTimeZone )
		{
			switch ( currentTimeZone.PlayerIndex )
			{
				case 0:
					_leftScores++;
					Debug.Log( $"left goal -> {_leftScores}" );
					_view.MatchView.SetLeftScore( _leftScores );
					break;

				case 1:
					_rightScores++;
					Debug.Log( $"right goal -> {_rightScores}" );
					_view.MatchView.SetRightScore( _rightScores );
					break;
			}
		}

		private void ShowResult()
		{
			var winStatus = 0;
			winStatus = IsWin ? 1 : winStatus;
			winStatus = IsLose ? -1 : winStatus;
			
			_view.SwitchToResultView();
			_view.MatchView.ShowResultText( _view.BetView.BetCommand, winStatus );
		}

		private void CleanUp()
		{
		}
	}
}