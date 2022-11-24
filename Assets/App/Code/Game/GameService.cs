namespace App.Code.Game
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using App.Code.Utils;
	using UniRx;
	using UnityEngine;
	using Random = UnityEngine.Random;

	public class GameService : MonoBehaviour
	{
		public struct PicData
		{
			public int Index;
			public int Type;
		}

		private IEnumerable<IObservable<RowState>> _rowStatesO;
		
		private GameView _view;
		private GameSettings _settings;
		private List<List<PicData>> _winCombos;
		private List<List<PicData>> _loseCombos;
		private List<PicData> _currentCombo;
		private List<Pic> _winPics;
		private List<Pic> _markedPics;
		private int _comboOffset;

		public void Construct(GameView view, GameSettings settings) 
		{
			_view = view;
			_settings = settings;

			for ( var i = 0; i < _view.Rows.Count; i++ )
			{
				var row = _view.Rows[i];
				row.Construct( i, _settings );
			}

			_winPics = new List<Pic>();
			_markedPics = new List<Pic>();
		}

		private bool CanSpin => _view.Rows.All( row => row.CanSpin );
		private int IndexWithOffset( int i, int o ) => (i - o + Row.Total ) % (Row.Total );
		private int IndexWithOffsetF( int i, int o ) => (i + o - _comboOffset + Row.Total) % (Row.Total);

		public void Run()
		{
			_view.Show();

			InitCombos();
			
			_view.SpinButtonClick
				.Subscribe( _ => OnSpinButtonClick() )
				.AddTo( this );

			_rowStatesO = _view.Rows
				.Select( row => row.State.Where( _ => _view.Rows.All( row2 => row2.State.Value == RowState.Stopped ) ).Skip( 1 ) );
			
			_rowStatesO
				.Merge()
				.Subscribe( _ => OnAllStopped() )
				.AddTo( this );
		}

		public List<PicData> GetRandomCombo()
		{
			var rnd = Random.Range( 0f, 1f );
			var isWin = rnd <= _settings.WinChance;

			var combos = isWin ? _winCombos : _loseCombos;
			var index = Random.Range( 0, combos.Count );
			return combos[index];
		}

		private void InitCombos()
		{
			var combos = GetAllCombos( _view.Rows );

			_winCombos = combos.Where( combo => combo.All( elem => elem.Type == combo[0].Type )).ToList();
			_loseCombos = combos.Where( combo => !_winCombos.Contains( combo )).ToList();
			
			Debug.Log( $"Win combos : {_winCombos.Count}, Lose combos : {_loseCombos.Count}" );
		}

		private void OnSpinButtonClick()
		{
			if ( CanSpin )
			{
				StartCoroutine( SpinAsync() );
			}
			else
			{
				Debug.LogWarning( $"You can not spin now" );
			}
		}

		private IEnumerator SpinAsync()
		{
			var delay = Mathf.Max( _settings.AccelerateDuration + 0.1f, _settings.DelayBeforeStop );
			Debug.LogWarning( $"SPIN! delay : {delay}" );

			StartSpin();

			yield return new WaitForSeconds( delay );
			StopSpin();
		}

		private void StartSpin()
		{
			StopWinFX();

			_currentCombo = GetRandomCombo();
			_comboOffset = Random.Range( 0, 3 );
			
			_currentCombo.ForEach( c => Debug.Log( $"{c.Index}:{c.Type}, OFFSET: {_comboOffset}" ) );
			
			foreach ( var row in _view.Rows )
			{
				row.StartSpin( _settings.AccelerateDuration, _settings.MaxSpeed );
			}

			if ( _settings.EnableMarks )
			{
				ClearMarks();
				SetMarks();
			}
		}

		private void StopSpin()
		{
			//var indexes = _currentCombo.Select( combo => combo.Index).ToArray();
			var indexes = _currentCombo.Select( combo => IndexWithOffset( combo.Index, _comboOffset ) ).ToArray();

			for ( var i = 0; i < _view.Rows.Count; i++ )
			{
				var row = _view.Rows[i];
				row.Brake( indexes[i], _settings.DecelerateDuration, _settings.MaxFinishingSpeed );
			}
		}

		[ContextMenu("PrintAllCombos")]
		private void PrintAllCombos()
		{
			var combos = GetAllCombos( _view.Rows );

			var result = $"TOTAL COMBOS : {combos.Count}\n";
			combos.ForEach( combo =>
			{
				var isRewarded = combo.All( elem => elem.Type == combo[0].Type );
				var col = isRewarded ? "yellow" : "green";
				result = $"{result}<color={col}>| ";
				combo.ForEach( elem => result = $"{result}{elem:D2} |" );
				result = $"{result}</color>\n";
			} );
			Debug.Log( $"{result}" );
		}

		List<List<PicData>> GetAllCombos( IEnumerable<Row> rows )
		{
			var rowArray = rows.ToArray();
			var combos = new List<List<PicData>>();
			var rowIndex = 0;
			rowIndex = 0;

			while ( IsValidRow( rowIndex ) )
			{
				var newCombos = new List<List<PicData>>();

				for ( int line = 0; line < Row.Total; line++ )
				{
					if ( combos.Count == 0 )
					{
						newCombos.Add( new List<PicData> {RowLineFunc( rowIndex, line )} );
					}
					else
					{
						for ( var i = 0; i < combos.Count; i++ )
						{
							var combo = combos[i];
							var newCombo = new List<PicData>( combo );
							newCombo.Add( RowLineFunc( rowIndex, line ) );
							newCombos.Add( newCombo );
						}
					}
				}

				combos = newCombos;
				rowIndex++;
			}

			return combos;

			bool IsValidRow( int curRow ) => curRow < rowArray.Length && curRow >= 0;

			PicData RowLineFunc( int row, int line )
			{
				return new() {Index = line, Type = rowArray[row].Pics[line].ID};
			}
		}

		[ContextMenu("Set Force Indexes")]
		private void SetForceIndexes()
		{
			for ( var i = 0; i < _currentCombo.Count; i++ )
			{
				var data = _currentCombo[i];
				var index = IndexWithOffset( data.Index, _comboOffset);
				_view.Rows[i].SetForceIndex( index );
			}
		}

		private void OnAllStopped()
		{
			Debug.Log( "ALL STOPPED" );
			CheckIfWinAndPlayFX();
		}

		private void CheckIfWinAndPlayFX()
		{
			var rows = _view.Rows;
			
			for ( int offset = 0; offset < 3; offset++ )
			{
				var indexes = _currentCombo.Select( data => IndexWithOffsetF( data.Index, offset ) ).ToArray();

				var pics = new List<Pic>();
				var picsDoubles = new List<Pic>();
				indexes.ForEach( (index, row) =>
				{
					pics.Add( rows[row].Pics[index] );
					
					if(index < Row.PicsVisible)
						picsDoubles.Add( rows[row].Pics[index + Row.Total] );
				} );

				var isWin = pics.All( pic => pic.ID == pics[0].ID );

				if ( isWin )
				{
					pics.ForEach( pic =>
					{
						_winPics.Add( pic );
						pic.SetWinFXState( true );
					} );

					picsDoubles.ForEach( pic =>
					{
						_winPics.Add( pic );
						pic.SetWinFXState( true );
					} );
				}
			}
		}

		private void StopWinFX()
		{
			_winPics.ForEach( pic => pic.SetWinFXState( false ));
			_winPics.Clear();
		}

		private void SetMarks()
		{
			var rows = _view.Rows;

			for ( int offset = 0; offset < 3; offset++ )
			{
				var indexes = _currentCombo.Select( data => IndexWithOffsetF( data.Index, offset ) ).ToArray();

				var pics = new List<Pic>();
				indexes.ForEach( ( index, row ) =>
				{
					pics.Add( rows[row].Pics[index] );

					if ( index < Row.PicsVisible )
						pics.Add( rows[row].Pics[index + Row.Total] );
				} );

				pics.ForEach( pic =>
				{
					_markedPics.Add( pic );
					pic.SetMarkState( true );
				} );
			
			}
		}

		private void ClearMarks()
		{
			_markedPics.ForEach( pic => pic.SetMarkState( false ) );
			_markedPics.Clear();
		}
	}
}