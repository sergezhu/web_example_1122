namespace App.Code.Game
{
	using TMPro;
	using UnityEngine;

	public class LedView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _scoresText;

		public void SetScores( int current, int max )
		{
			_scoresText.text = $"{current} / {max}";
		}
	}
}