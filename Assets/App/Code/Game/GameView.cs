namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using App.Code.UI;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : BaseUIWindow
	{
		[SerializeField] private UINoInternetWarningWindow _noInternetWarningWindow;
		
		[Space]
		[SerializeField] private Button _spinButton;

		[Space]
		[SerializeField] private List<Row> _rows;

		public IObservable<Unit> SpinButtonClick { get; private set; }
		public IReadOnlyList<Row> Rows => _rows;
		

		private void Awake()
		{
			Hide();
			
			SpinButtonClick = _spinButton.onClick.AsObservable();

			_noInternetWarningWindow.Hide();
			_noInternetWarningWindow.Init();
		}

		public override void Init()
		{
		}

		public void ShowNoInternetWindow() => _noInternetWarningWindow.Show();
		public void HideNoInternetWindow() => _noInternetWarningWindow.Hide();
		
	}
}