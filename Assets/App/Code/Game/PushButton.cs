namespace App.Code.Game
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class PushButton : MonoBehaviour
	{
		[SerializeField] private Button _button;
		[SerializeField] private Image _img;

		public IObservable<Unit> ButtonClick { get; private set; }
		public int Index { get; set; }
		public bool Enable { get; set; }

		public void Init()
		{
			ButtonClick = _button.onClick.AsObservable().Where( _ => Enable );
		}

		public void SetSprite( Sprite sprite )
		{
			_img.sprite = sprite;
		}
	}
}