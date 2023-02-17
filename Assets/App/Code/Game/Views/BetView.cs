namespace App.Code.Game
{
	using System;
	using App.Code.Game.Enums;
	using global::Game.Code.UI.Button;
	using UniRx;
	using UnityEngine;

	public class BetView : MonoBehaviour
	{
		[SerializeField] private UIBaseButton _leftSelect;
		[SerializeField] private UIBaseButton _rightSelect;

		public IObservable<Unit> LeftCommandSelect { get; private set; }
		public IObservable<Unit> RightCommandSelect { get; private set; }
		public ECommand BetCommand { get; private set; }

		public void Initialize()
		{
			_leftSelect.Initialize();
			_rightSelect.Initialize();

			LeftCommandSelect = _leftSelect.Click;
			RightCommandSelect = _rightSelect.Click;

			LeftCommandSelect
				.Subscribe( _ => DoSelectLeft() )
				.AddTo( this );

			RightCommandSelect
				.Subscribe( _ => DoSelectRight() )
				.AddTo( this );
		}

		public void ResetCheckboxes()
		{
			_leftSelect.IsSelected.Value = false;
			_rightSelect.IsSelected.Value = false;
		}

		private void DoSelectRight()
		{
			BetCommand = ECommand.Right;

			_leftSelect.IsSelected.Value = false;
			_rightSelect.IsSelected.Value = true;

			Debug.Log( "SelectRight" );
		}

		private void DoSelectLeft()
		{
			BetCommand = ECommand.Left;

			_leftSelect.IsSelected.Value = true;
			_rightSelect.IsSelected.Value = false;

			Debug.Log( "SelectLeft" );
		}
	}
}