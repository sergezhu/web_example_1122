namespace App.Code.Game
{
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public class KeglesController : MonoBehaviour
	{
		[SerializeField] private List<Kegl> _kegles;
		
		private GameSettings _settings;
		private CompositeDisposable _disposables;

		public int KeglesCount { get; private set; }
		public ReactiveProperty<int> KeglesFaultCount { get; } = new ReactiveProperty<int>();

		public void Construct( GameSettings settings )
		{
			_settings = settings;
			_disposables = new CompositeDisposable();
			
			_kegles.ForEach( k => k.Construct( settings ) );

			KeglesCount = _kegles.Count;
			
			_kegles.ForEach( k =>
			{
				k.IsFault
					.Subscribe( _ => KeglesFaultCount.Value += 1 )
					.AddTo( _disposables );
			} );
		}

		public void Initialize()
		{
			KeglesFaultCount.Value = 0;
			
			_kegles.ForEach( k => k.Initialize() );
		}
	}
}