namespace App.Code.Game
{
	using System;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class WordButton : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _wordText;
		[SerializeField] private Button _button;
		[SerializeField] private Image _img;

		public IObservable<Unit> ButtonClick;
		public IObservable<int> IndexedClick { get; private set; }
		public bool Enable { get; set; }
		public int Index { get; set; }

		public void Init()
		{
			ButtonClick = _button.onClick.AsObservable().Where( _ => Enable );
			IndexedClick = ButtonClick.Select( _ => Index );
		}

		public void SetText( string t )
		{
			_wordText.text = t;
		}

		public void SetSprite( Sprite sprite )
		{
			_img.sprite = sprite;
		}
	}
}