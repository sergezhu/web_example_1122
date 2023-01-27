namespace App.Code.Game
{
	using UniRx;
	using UnityEngine;

	public class BowlVeil : MonoBehaviour
	{
		public ReactiveCommand Closed { get; } = new ReactiveCommand();
		public ReactiveCommand Opened { get; } = new ReactiveCommand();

		public void Close()
		{
			
		}

		public void Open()
		{
			
		}
	}
}