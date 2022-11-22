namespace App.Code.Game
{
	using System;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameView : MonoBehaviour
	{
		[SerializeField] private Button _spinButton;

		[Space]
		[SerializeField] private List<Row> _rows;

		public IObservable<Unit> SpinButtonClick { get; private set; }
		public IReadOnlyList<Row> Rows => _rows;

		private void Awake()
		{
			SpinButtonClick = _spinButton.onClick.AsObservable();
		}
	}
}