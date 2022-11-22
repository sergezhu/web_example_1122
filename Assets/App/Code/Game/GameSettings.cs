namespace App.Code.Game
{
	using UnityEngine;

	public class GameSettings : MonoBehaviour
	{
		public float AccelerateDuration;
		public float DecelerateDuration;
		public float MaxSpeed;
		public float MinSpeed;
		public float SpeedThreshold;
		public float DelayBeforeStop;

		[Space]
		public int[] TargetIndex;
		public int LoopsBeforeStop;
	}
}