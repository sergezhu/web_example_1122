namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using App.Code.FX;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : BaseUIWindow
	{
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		[SerializeField] private UINoDataWarningWindow _noDataWarningWindow;

		[Space]
		[SerializeField] private GameObject _wordInfo;
		[SerializeField] private List<WordButton> _wordButtons;
		[SerializeField] private NextButton _nextButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;
		
		private GameSettings _settings;

		public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<int> WordButtonClick { get; private set; }
		public IObservable<Unit> ExitButtonClick { get; private set; }

		public List<WordButton> WordButtons => _wordButtons;

		public void Construct( GameSettings settings )
		{
			_settings = settings;
		}

		public override void Init()
		{
			Hide();

			ExitButtonClick = _exitButton.onClick.AsObservable();
			
			_nextButton.Init();
			NextButtonClick = _nextButton.ButtonClick;

			_wordButtons.ForEach( b => b.Init() );
			var wordClicks = _wordButtons.Select( b => b.IndexedClick );
			WordButtonClick = wordClicks.Merge();

			_noInternetWarningWindow.Hide();
			_noInternetWarningWindow.Init();
		}

		public void EnableWordsButtons()
		{
			_wordButtons.ForEach( b => b.Enable = true );
		}

		public void DisableWordsButtons()
		{
			_wordButtons.ForEach( b => b.Enable = false );
		}
		

		public void EnableNextButton()
		{
			_wordInfo.SetActive( false );
			_nextButton.Enable = true;
			_nextButton.gameObject.SetActive( true );
			//_nextButton.SetSprite( _settings.NextDefaultState );
		}

		public void DisableNextButton()
		{
			_wordInfo.SetActive( true );
			_nextButton.Enable = false;
			_nextButton.gameObject.SetActive( false );
			//_nextButton.SetSprite( _settings.NextInactiveState );
		}

		public void ShowNoInternetWindow() => _noInternetWarningWindow.Show();
		public void HideNoInternetWindow() => _noInternetWarningWindow.Hide();
		public void ShowNoDataWindow() => _noDataWarningWindow.Show();
		public void HideNoDataWindow() => _noDataWarningWindow.Hide();
		public void SetResultNoDataWindow(string result) => _noDataWarningWindow.SetResultText( result );

		public void SetCoinsEmission( float value )
		{
			_coinsLeftFX.SetEmission( value );
			_coinsRightFX.SetEmission( value );
		}

		public void PlayCoinsFX()
		{
			_coinsLeftFX.PlayProperly();
			_coinsRightFX.PlayProperly();
		}

		public void StopCoinsFX()
		{
			_coinsLeftFX.Stop();
			_coinsRightFX.Stop();
		}
	}
}