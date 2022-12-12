namespace App.Code.Game
{
	using System.Collections.Generic;
	using App.Code.Utils;
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
			GenerateWords();
			
			_view.Show();
			
			_view.NextButtonClick
				.Subscribe( _ => OnNextButtonClick())
				.AddTo( this );

			_view.WordButtonClick
				.Subscribe( i => OnWordButtonClick( i ) )
				.AddTo( this );

			_view.ExitButtonClick
				.Subscribe( _ => Application.Quit() )
				.AddTo( this );

			StartGame();
			GameStarted.Execute();
		}

		public void StartGame()
		{
			var data = GetRandomWords();
			
			_currentWords = data.Item1;
			_currentRightIndex = data.Item2;
			
			PrintRandomWords(_currentWords);
			Debug.Log( $"_currentRightIndex : {_currentRightIndex}" );
			
			ButtonsInitialize();
		}

		public void ButtonsInitialize()
		{
			_view.WordButtons.ForEach( ( button, i ) =>
			{
				button.Index = i;
				button.SetSprite( _settings.DefaultState );
				button.SetText( _currentWords[i] );
				button.Enable = true;
			} );

			_view.DisableNextButton();
		}

		private void OnNextButtonClick()
		{
			CleanUp();
			StartGame();
		}

		private void CleanUp()
		{
		}

		private void OnWordButtonClick(int index)
		{
			Debug.Log( $"Word selected : {index}" );
			
			_view.WordButtons.ForEach( ( button, i ) =>
			{
				var sprite = i != index && i != _currentRightIndex
					? _settings.InactiveState
					: i == _currentRightIndex
						? _settings.RightState
						: _settings.FalseState;
				
				button.SetSprite( sprite );
				button.Enable = false;
			} );

			WordSelected.Execute( _currentRightIndex == index );
			
			_view.EnableNextButton();
		}

		private void GenerateWords()
		{
			_sportWords = _settings.SportWords.Split( ',' );
			_nonSportWords = _settings.NonSportWords.Split( ',' );
		}

		private (string[], int) GetRandomWords()
		{
			var size = _settings.WordsGroupSize;
			var result = new List<string>();
			var indexesS = new List<int>();
			var indexesNS = new List<int>();

			var rightWordIndex = Random.Range( 0, size );

			for ( int i = 0; i < size; i++ )
			{
				int index;
				var indexes = rightWordIndex == i ? indexesS : indexesNS;

				do
				{
					index = rightWordIndex == i
						? Random.Range( 0, _nonSportWords.Length )
						: Random.Range( 0, _sportWords.Length );
				} 
				while ( indexes.Contains( index ) );

				string w = rightWordIndex == i
					? _nonSportWords[index]
					: _sportWords[index];
				
				indexes.Add( index );
				result.Add( w.ToUpper() );
			}

			return (result.ToArray(), rightWordIndex);
		}

		[ContextMenu("Print Random Words")]
		private void PrintRandomWords()
		{
			var words = GetRandomWords();

			PrintRandomWords( words.Item1 );
		}

		private static void PrintRandomWords( string[] words )
		{
			var wordsStr = "WORDS: ";
			words.ForEach( w => wordsStr = $"{wordsStr}, {w}" );
			Debug.Log( wordsStr );
		}
	}
}