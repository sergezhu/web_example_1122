namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using App.Code.FX;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	public class GameView : BaseUIWindow
	{
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		[SerializeField] private UINoDataWarningWindow _noDataWarningWindow;

		[FormerlySerializedAs( "_nextButton" )]
		[Space]
		[SerializeField] private PushButton _pushButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;
		
		private GameSettings _settings;

		public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<Unit> ExitButtonClick { get; private set; }
		public IObservable<Unit> AnyButtonClick { get; private set; }

		public void Construct( GameSettings settings )
		{
			_settings = settings;
		}

		public override void Init()
		{
			Hide();

			ExitButtonClick = _exitButton.onClick.AsObservable();
			
			_pushButton.Init();
			NextButtonClick = _pushButton.ButtonClick;

			_noInternetWarningWindow.Hide();
			_noInternetWarningWindow.Init();
		}

		public void EnableNextButton()
		{
			_pushButton.Enable = true;
			_pushButton.gameObject.SetActive( true );
			//_nextButton.SetSprite( _settings.NextDefaultState );
		}

		public void DisableNextButton()
		{
			_pushButton.Enable = false;
			_pushButton.gameObject.SetActive( false );
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