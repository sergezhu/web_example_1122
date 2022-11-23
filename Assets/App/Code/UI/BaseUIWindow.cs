namespace App.Code.UI
{
	using UniRx;
	using UnityEngine;

	public abstract class BaseUIWindow : MonoBehaviour
	{
		public ReactiveCommand Shown { get; } = new ReactiveCommand();
		public ReactiveCommand Hidden { get; } = new ReactiveCommand();

		private bool IsShown => gameObject.activeSelf;

		public abstract void Init();
	
		public void Show(bool silent = false)
		{
			if ( IsShown )
				return;
			
			gameObject.SetActive( true );

			if ( !silent )
			{
				Shown.Execute();
			}
		}

		public void Hide( bool silent = false )
		{
			if ( !IsShown )
				return;
			
			gameObject.SetActive( false );

			if ( !silent )
			{
				Hidden.Execute();
			}
		}
	}
}