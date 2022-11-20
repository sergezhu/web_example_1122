namespace App.Code.Loader
{
	using UniRx;
	using UnityEngine;

	public class InternetStateService
	{
		private readonly ReactiveProperty<bool> _hasInternet = new();

		public InternetStateService()
		{
			HasInternet = _hasInternet.ToReadOnlyReactiveProperty();
		}

		public ReadOnlyReactiveProperty<bool> HasInternet { get; } 
		
		public bool Check()
		{
			return _hasInternet.Value = Application.internetReachability != NetworkReachability.NotReachable;
		}
	}
}