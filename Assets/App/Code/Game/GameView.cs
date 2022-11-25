namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using App.Code.FX;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : BaseUIWindow
	{
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		
		[Space]
		[SerializeField] private Button _spinButton;
		[SerializeField] private Button _exitButton;

		[Space]
		[SerializeField] private List<Row> _rows;

		[Space]
		[SerializeField] private FXWrapper _coinsLeftFX;
		[SerializeField] private FXWrapper _coinsRightFX;

		public IObservable<Unit> SpinButtonClick { get; private set; }
		public IObservable<Unit> ExitButtonClick { get; private set; }
		public IReadOnlyList<Row> Rows => _rows;


		public override void Init()
		{
			Hide();

			SpinButtonClick = _spinButton.onClick.AsObservable();
			ExitButtonClick = _exitButton.onClick.AsObservable();

			_noInternetWarningWindow.Hide();
			_noInternetWarningWindow.Init();
		}

		public void ShowNoInternetWindow() => _noInternetWarningWindow.Show();
		public void HideNoInternetWindow() => _noInternetWarningWindow.Hide();

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