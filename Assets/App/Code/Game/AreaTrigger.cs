namespace App.Code.Game
{
	using UniRx;
	using UnityEngine;

	[RequireComponent(typeof(Collider))]
	public class AreaTrigger : MonoBehaviour
	{
		public ReactiveProperty<bool> IsInside { get; } = new ReactiveProperty<bool>();

		private void OnTriggerEnter( Collider other )
		{
			IsInside.Value = true;
		}

		private void OnTriggerExit( Collider other )
		{
			IsInside.Value = false;
		}
	}
}