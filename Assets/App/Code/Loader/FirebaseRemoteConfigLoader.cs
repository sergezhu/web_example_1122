namespace App.Code.Loader
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Firebase.Extensions;
	using Firebase.RemoteConfig;
	using UnityEngine;

	public  class FirebaseRemoteConfigLoader
	{
		public enum FirebaseStatus
		{
			Unknown,
			Failed,
			Connected,
			IsReady
		}


		private readonly Action<string> _onErrorAction;
		private readonly FirebaseRemoteConfig _remoteConfig;

		public FirebaseStatus FbStatus { get; private set; } = FirebaseStatus.Unknown;


		public FirebaseRemoteConfigLoader( Action<string> onErrorAction = null )
		{
			_onErrorAction = onErrorAction;
			_remoteConfig = FirebaseRemoteConfig.DefaultInstance;

			Debug.Log( $"FirebaseRemoteConfig Initialized" );
		}

		public void SetDefault( Dictionary<string, object> data)
		{
			_remoteConfig.SetDefaultsAsync( data ).ContinueWithOnMainThread( task =>
			{
				Debug.Log( "Save Default" );
				try
				{
					Fetch();
				}
				catch ( Exception ex )
				{
					Debug.LogError( $"Firebase Fetching Exception : {ex.Message}" );
					_onErrorAction?.Invoke( ex.Message );
				}
			} );
		}

		public Task Fetch()
		{
			Debug.Log( "Fetching data..." );
			Task fetchTask = _remoteConfig.FetchAsync( TimeSpan.Zero );

			return fetchTask.ContinueWithOnMainThread( FetchComplete );
		}

		void FetchComplete( Task fetchTask )
		{
			if ( fetchTask.IsCanceled )
			{
				Debug.Log( "Fetch canceled." );
				FbStatus = FirebaseStatus.Failed;
			}
			else if ( fetchTask.IsFaulted )
			{
				Debug.Log( "Fetch encountered an error." );
				FbStatus = FirebaseStatus.Failed;
			}
			else if ( fetchTask.IsCompleted )
			{
				Debug.Log( "Fetch completed successfully!" );
				FbStatus = FirebaseStatus.Connected;
			}

			var info = _remoteConfig.Info;
			string errMsg;
			switch ( info.LastFetchStatus )
			{
				case LastFetchStatus.Success:
					_remoteConfig.ActivateAsync()
						.ContinueWithOnMainThread( task =>
						{
							FbStatus = FirebaseStatus.IsReady;
							Debug.Log( String.Format( "Remote data loaded and ready (last fetch time {0})", info.FetchTime ) );
						} );
					break;
				
				case LastFetchStatus.Failure:
					FbStatus = FirebaseStatus.IsReady;
					switch ( info.LastFetchFailureReason )
					{
						case FetchFailureReason.Error:
							errMsg = "Fetch failed for unknown reason";
							Debug.Log( errMsg );
							_onErrorAction?.Invoke( errMsg );
							break;
						case FetchFailureReason.Throttled:
							errMsg = "Fetch throttled until " + info.ThrottledEndTime;
							Debug.Log( errMsg );
							_onErrorAction?.Invoke( errMsg );
							break;
					}
					break;
				
				case LastFetchStatus.Pending:
					FbStatus = FirebaseStatus.IsReady;
					errMsg = "Latest Fetch call still pending.";
					Debug.Log( errMsg );
					_onErrorAction?.Invoke( errMsg );
					break;
			}
		}

		public ConfigValue ReadValue( string key )
		{
			return _remoteConfig.GetValue( key );
		}
	}
}