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

		private IObservable<Unit> _buttonClick;
		public IObservable<int> IndexedClick { get; private set; }
		public bool Enable { get; set; }
		public int Index { get; set; }

		public void Init()
		{
			_buttonClick = _button.onClick.AsObservable().Where( _ => Enable );
			IndexedClick = _buttonClick.Select( _ => Index );
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