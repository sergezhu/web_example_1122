namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using App.Code.FX;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameObject _gameCanvasRoot;
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		[SerializeField] private UINoDataWarningWindow _noDataWarningWindow;

		[Space]
		[SerializeField] private UIButton _nextButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;
		
		private GameSettings _settings;

		//public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<Unit> ExitButtonClick { get; private set; }
		public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<Unit> AnyButtonClick { get; private set; }
		

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

			var anyClicksObservables = new List<IObservable<Unit>>() { ExitButtonClick, NextButtonClick };
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