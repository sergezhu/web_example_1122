namespace App.Code.UI
{
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class UINoDataWarningWindow : BaseUIWindow
	{
		[SerializeField] private Button _exitButton;
		[SerializeField] private TMP_Text _resultText;

		public ReactiveCommand ExitClick { get; } = new ReactiveCommand();

		public override void Init()
		{
			if ( gameObject.activeSelf )
				Shown.Execute();

			Subscribe();
		}

		public void SetResultText( string result )
		{
			_resultText.text = $"Fetching data is failed. Reason:\n{result}";
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