namespace App.Code.Game
{
	using UnityEngine;

	public class GameSettings : MonoBehaviour
	{
		public float AccelerateDuration;
		public float DecelerateDuration;
		public float MaxSpeed;
		public float MinFinishingSpeed;
		public float MaxFinishingSpeed;
		public float SpeedThreshold;
		public float DelayBeforeStop;
		public float RandomDelayBeforeStart;
		public int LoopsBeforeStop;

		[Header( "Random Tune" )]
		[Range(0, 1)]
		public float WinChance;

		[Header( "Debug" )]
		public bool EnableMarks;
	}
}