namespace App.Code.Game
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class UIButton : MonoBehaviour
	{
		[SerializeField] private Button _button;

		public IObservable<Unit> ButtonClick { get; private set; }
		public bool Enable { get; set; }

		public void Init()
		{
			ButtonClick = _button.onClick.AsObservable().Where( _ => Enable );
		}
	}
}