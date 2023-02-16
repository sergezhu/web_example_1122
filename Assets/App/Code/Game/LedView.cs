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

		public void SetScores( int left, int right )
		{
			_scoresText.text = $"{left} : {right}";
		}
	}
}