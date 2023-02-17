namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using App.Code.FX;
	using App.Code.UI;
	using App.Code.Utils;
	using global::Game.Code.UI.Button;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameObject _gameCanvasRoot;
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		[SerializeField] private UINoDataWarningWindow _noDataWarningWindow;

		[Space]
		[SerializeField] private UIButton _startButton;
		[SerializeField] private UIButton _nextButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private BetView _betView;
		[SerializeField] private MatchView _matchView;
		[SerializeField] private FlagsView _flagsView;
		[SerializeField] private ResultView _resultView;
		[SerializeField] private SpriteRandomizer _flagLeftRandomizer;
		[SerializeField] private SpriteRandomizer _flagRightRandomizer;
		[SerializeField] private TextMeshProUGUI _hintText;

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;
		
		private GameSettings _settings;

		public IObservable<Unit> ExitButtonClick { get; private set; }
		public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<Unit> StartButtonClick { get; private set; }
		public IObservable<Unit> AnyButtonClick { get; private set; }

		public BetView BetView => _betView;
		public MatchView MatchView => _matchView;
		public FlagsView FlagsView => _flagsView;
		public ResultView ResultView => _resultView;

		public SpriteRandomizer FlagLeftRandomizer => _flagLeftRandomizer;
		public SpriteRandomizer FlagRightRandomizer => _flagRightRandomizer;


		public void Construct( GameSettings settings )
		{
			_settings = settings;
		}

		public void Init()
		{
			Hide();

			ExitButtonClick = _exitButton.onClick.AsObservable();
			
			_nextButton.Init();
			NextButtonClick = _nextButton.ButtonClick;
			
			_startButton.Init();
			StartButtonClick = _startButton.ButtonClick;

			_betView.Initialize();
			
			var anyClicksObservables = new List<IObservable<Unit>>() { ExitButtonClick, NextButtonClick, _betView.LeftCommandSelect, _betView.RightCommandSelect };
			AnyButtonClick = anyClicksObservables.Merge();

			_noInternetWarningWindow.Hide();
			_noInternetWarningWindow.Init();
		}

		public void Hide()
		{
			gameObject.SetActive( false );
			_gameCanvasRoot.gameObject.SetActive( false );
		}

		public void Show()
		{
			gameObject.SetActive( true );
			_gameCanvasRoot.gameObject.SetActive( true );
		}

		public void EnableNextButton()
		{
			_nextButton.Enable = true;
			_nextButton.gameObject.SetActive( true );
			//_nextButton.SetSprite( _settings.NextDefaultState );
		}

		public void DisableNextButton()
		{
			_nextButton.Enable = false;
			_nextButton.gameObject.SetActive( false );
			//_nextButton.SetSprite( _settings.NextInactiveState );
		}

		public void ShowNoInternetWindow() => _noInternetWarningWindow.Show();
		public void HideNoInternetWindow() => _noInternetWarningWindow.Hide();
		public void ShowNoDataWindow() => _noDataWarningWindow.Show();
		public void HideNoDataWindow() => _noDataWarningWindow.Hide();
		public void SetResultNoDataWindow(string result) => _noDataWarningWindow.SetResultText( result );


		public void SwitchToBetSView()
		{
			BetView.Enable();
			FlagsView.Disable();
			MatchView.Disable();
			//_view.ResultView.Disable();

			_startButton.Enable();
			_nextButton.Disable();
			
			_hintText.Enable();
			_hintText.text = "Guess the winner";
		}

		public void SwitchToMatchView()
		{
			BetView.Disable();
			FlagsView.Enable();
			MatchView.Enable();
			//_view.ResultView.Disable();

			_startButton.Disable();
			_nextButton.Disable();

			_hintText.Enable();
			_hintText.text = "Match is going...";
		}

		public void SwitchToResultView()
		{
			BetView.Disable();
			FlagsView.Enable();
			MatchView.Disable();
			ResultView.Enable();

			_startButton.Disable();
			_nextButton.Enable();

			_hintText.Disable();
		}

		/*public void SetCoinsEmission( float value )
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
		}*/
	}
}