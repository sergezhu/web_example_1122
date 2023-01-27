namespace App.Code.Game
{
	using TMPro;
	using UnityEngine;

	public class LedView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _scoresText;
		private GameSettings _settings;

		public void Construct( GameSettings settings )
		{
			_settings = settings;
		}

		public void SetScores( int current, int max )
		{
			var scoreColor = current >= _settings.WinKeglesCount ? Color.green : Color.red;
			_scoresText.color = scoreColor;
			
			_scoresText.text = $"{current} / {max}";
		}
	}
}