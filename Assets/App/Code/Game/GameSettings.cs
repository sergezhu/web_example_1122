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

		[Space]
		public int TargetIndex;
		public int LoopsBeforeStop;
	}
}