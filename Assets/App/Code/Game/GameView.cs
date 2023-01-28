namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using App.Code.FX;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	public class GameView : MonoBehaviour
	{
		[SerializeField] private GameObject _gameCanvasRoot;
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		[SerializeField] private UINoDataWarningWindow _noDataWarningWindow;

		[FormerlySerializedAs( "_nextButton" )]
		[Space]
		[SerializeField] private UIButton _pushButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private AreaTrigger _finishTrigger;
		[SerializeField] private AreaTrigger _nearFinishTrigger;

		[Space]
		[SerializeField] private LedView _ledView;
		[SerializeField] private ArrowBlock _arrowBlock;
		[SerializeField] private Ball _ball;
		[SerializeField] private BowlVeil _bowlVeil;
		

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;
		
		private GameSettings _settings;

		//public IObservable<Unit> NextButtonClick { get; private set; }
		public IObservable<Unit> ExitButtonClick { get; private set; }
		public IObservable<Unit> PushButtonClick { get; private set; }
		public IObservable<Unit> AnyButtonClick { get; private set; }
		public IObservable<bool> NearFinishEnter { get; private set; }
		public IObservable<bool> FinishEnter { get; private set; }
		public LedView LedView => _ledView;
		public Ball Ball => _ball;
		public ArrowBlock ArrowsBlock => _arrowBlock;
		public BowlVeil BowlVeil => _bowlVeil;

		public void Construct( GameSettings settings )
		{
			_settings = settings;
			_ball.Construct();
			_ledView.Construct( settings );
			_arrowBlock.Construct( settings );
		}

		public void Init()
		{
			Hide();

			ExitButtonClick = _exitButton.onClick.AsObservable();
			
			_pushButton.Init();
			PushButtonClick = _pushButton.ButtonClick;

			var anyClicksObservables = new List<IObservable<Unit>>() { ExitButtonClick, PushButtonClick };
			AnyButtonClick = anyClicksObservables.Merge();

			NearFinishEnter = _nearFinishTrigger.IsInside.Where( v => v );
			FinishEnter = _finishTrigger.IsInside.Where( v => v );

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

		public void EnablePushButton()
		{
			_pushButton.Enable = true;
			_pushButton.gameObject.SetActive( true );
			//_nextButton.SetSprite( _settings.NextDefaultState );
		}

		public void DisablePushButton()
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

		public void ShowArrowsBlock() => _arrowBlock.Show();
		public void HideArrowsBlock() => _arrowBlock.Hide();

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