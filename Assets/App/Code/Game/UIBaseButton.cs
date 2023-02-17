namespace Game.Code.UI.Button
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class UIBaseButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private GameObject _defaultView;
		[SerializeField] private GameObject _selectedView;
		[SerializeField] private GameObject _disabledView;
		
		public ReactiveCommand Click { get; } = new ReactiveCommand();
		public ReactiveProperty<bool> IsSelected { get; } = new ReactiveProperty<bool>();
		public ReactiveProperty<bool> IsEnabled { get; } = new ReactiveProperty<bool>();


		public void Initialize()
		{
			SubscribeInternal();

			IsEnabled.Value = true;
			IsSelected.Value = false;
		}

		public void OnPointerClick( PointerEventData eventData )
		{
			if ( !IsEnabled.Value )
				return;
			
			Debug.Log( $"OnPointerClickHandler : {this.name} : SCENE" );
			Click.Execute();
		}

		public void Show( bool force = false, Action onComplete = null )
		{
			gameObject.SetActive( true );
			onComplete?.Invoke();
		}

		public void Hide( bool force = false, Action onComplete = null )
		{
			gameObject.SetActive( false );
			onComplete?.Invoke();
		}

		private void SubscribeInternal()
		{
			IsSelected
				.Where( _ => IsEnabled.Value )
				.Subscribe( UpdateSelectedState )
				.AddTo( this );

			IsEnabled
				.Where( _ => _disabledView != null && _defaultView != null )
				.Subscribe( UpdateEnabledState )
				.AddTo( this );
		}

		private void UpdateSelectedState( bool v )
		{
			Debug.Log( $"update select state : {v}" );
			
			if(_selectedView != null)
				_selectedView.SetActive( v );
			
			if ( _defaultView != null )
				_defaultView.SetActive( !v );
		}

		private void UpdateEnabledState( bool v )
		{
			if ( v == false )
			{
				IsSelected.Value = false;
				
				if(_selectedView != null)
					_selectedView.SetActive( false );
			}
			else
			{
				if(IsSelected.Value && _selectedView != null )
					_selectedView.SetActive( true );
				else
					_defaultView.SetActive( true );
			}

			_disabledView.SetActive( !v );
		}
	}
}