namespace App.Code.Game
{
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public class KeglesController : MonoBehaviour
	{
		[SerializeField] private List<Kegl> _kegles;
		
		private GameSettings _settings;
		private bool _isLockEventFiring;

		private bool IsLockEventFiring => _isLockEventFiring;

		public int KeglesCount { get; private set; }
		public ReactiveProperty<int> KeglesFaultCount { get; } = new ReactiveProperty<int>();

		public void Construct( GameSettings settings )
		{
			_settings = settings;
			
			_kegles.ForEach( k => k.Construct( settings ) );

			KeglesCount = _kegles.Count;
			
			_kegles.ForEach( k =>
			{
				k.IsFault
					.Where( v => v && IsLockEventFiring == false )
					.Subscribe( _ =>
					{
						Debug.Log( $"{k.name} isFault = {k.IsFault.Value}" );
						KeglesFaultCount.Value += 1;
					} )
					.AddTo( this );
			} );
		}

		public void Initialize()
		{
			UnlockEventFiring();
			KeglesFaultCount.Value = 0;
			_kegles.ForEach( k => k.Initialize() );
		}

		public void ResetPositions()
		{
			_kegles.ForEach( k => k.ResetPosition() );
		}

		public void CleanUp()
		{
			_kegles.ForEach( k => k.CleanUp() );
		}

		public void LockEventFiring()
		{
			_isLockEventFiring = true;
		}

		public void UnlockEventFiring()
		{
			_isLockEventFiring = false;
		}
	}
}