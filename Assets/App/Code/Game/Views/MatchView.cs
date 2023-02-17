namespace App.Code.Game
{
	using System;
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
		[SerializeField] private GameObject _secondsMeter;
		[SerializeField] private TextMeshProUGUI _winText;
		[SerializeField] private TextMeshProUGUI _loseText;
		[SerializeField] private TextMeshProUGUI _standoffText;

		[Space]
		[SerializeField] private Transform _timeArrow;
		[SerializeField] private Image _timeCircle;

		[Space]
		[SerializeField] private Color _defaultColor;
		[SerializeField] private Color _winColor;
		[SerializeField] private Color _loseColor;
		
		private float _defaultArrowRotationZ;

		private void Awake()
		{
			_defaultArrowRotationZ = _timeArrow.eulerAngles.z;
			
			SetBetCommand( ECommand.Left );
		}


		public void SetBetCommand( ECommand command )
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

		public void ShowSecondsMeter() => _secondsMeter.Enable();
		public void HideSecondsMeter() => _secondsMeter.Disable();

		public void ShowResultText( ECommand command, int winStatus )
		{
			switch ( winStatus )
			{
				case -1:
					_winText.Disable();
					_loseText.Enable();
					_standoffText.Disable();
					break;
				
				case 0:
					_winText.Disable();
					_loseText.Disable();
					_standoffText.Enable();
					break;
				
				case 1:
					_winText.Enable();
					_loseText.Disable();
					_standoffText.Disable();
					break;
			}

			var color = winStatus == 1
				? _winColor
				: winStatus == -1
					? _loseColor
					: _defaultColor;

			switch ( command )
			{
				case ECommand.Left:
					_leftScores.color = color;
					break;
				case ECommand.Right:
					_rightScores.color = color;
					break;
			}
		}

		public void HideResultText()
		{
			_winText.Disable();
			_loseText.Disable();
		}

		public void ResetColors()
		{
			_leftScores.color = _defaultColor;
			_rightScores.color = _defaultColor;
		}

		public void SetLeftScore( int score ) => _leftScores.text = $"{score}";
		public void SetRightScore( int score ) => _rightScores.text = $"{score}";

		public void UpdateMatchTime( float time, float matchDuration )
		{
			var progress = time / matchDuration;

			_timeCircle.fillAmount = progress;
			_timeArrow.eulerAngles = Vector3.zero.WithZ( _defaultArrowRotationZ - 360f * progress );
		}
	}
}