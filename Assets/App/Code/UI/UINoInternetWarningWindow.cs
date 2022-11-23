namespace App.Code.UI
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class UINoInternetWarningWindow : BaseUIWindow
	{
		[SerializeField] private Button _exitButton;

		public ReactiveCommand ExitClick { get; } = new ReactiveCommand();

		public override void Init()
		{
			if ( gameObject.activeSelf )
				Shown.Execute();

			Subscribe();
		}

		private void Subscribe()
		{
			if ( _exitButton != null )
			{
				_exitButton.onClick
					.AsObservable()
					.Subscribe( _ => OnExitButtonClick() )
					.AddTo( this );
			}
		}

		private void OnExitButtonClick()
		{
			Hide();
			ExitClick.Execute();
			
			Debug.Log( "EXIT" );
			
			Application.Quit();
		}
	}
}