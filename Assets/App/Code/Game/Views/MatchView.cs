namespace App.Code.Game
{
	using App.Code.Game.Enums;
	using App.Code.Utils;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class MatchView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _leftScores;
		[SerializeField] private TextMeshProUGUI _rightScores;
		[SerializeField] private Image _leftFlagSelect;
		[SerializeField] private Image _rightFlagSelect;


		public void SetBettingCommand( ECommand command )
		{
			switch ( command )
			{
				case ECommand.Left:
					_leftFlagSelect.Enable();
					_rightFlagSelect.Disable();
					break;
				case ECommand.Right:
					_leftFlagSelect.Disable();
					_rightFlagSelect.Enable();
					break;
			}
		}

		public void SetLeftScore( int score ) => _leftScores.text = $"{score}";
		public void SetRightScore( int score ) => _rightScores.text = $"{score}";

		public void UpdateMatchTime( float time, float matchDuration )
		{
		}
	}
}