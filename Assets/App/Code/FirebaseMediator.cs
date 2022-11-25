namespace App.Code
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using App.Code.Loader;
	using Firebase;
	using UnityEngine;

	public class FirebaseMediator
	{
		private const string Key = "url";
		
		private InternetStateService _internetCheckService;
		private ConfiguredTaskAwaitable _initAwaitable;
		private FirebaseRemoteConfigLoader _remoteConfigLoader;
		private readonly Action<string> _onErrorAction;


		public FirebaseMediator( InternetStateService internetCheckService, FirebaseRemoteConfigLoader remoteConfigLoader, Action<string> onErrorAction = null )
		{
			_internetCheckService = internetCheckService;
			_remoteConfigLoader = remoteConfigLoader;
			_onErrorAction = onErrorAction;
		}

		public void Initialize()
		{
			//_initAwaitable = InitializeStorageReference().ConfigureAwait( false );
			InitializeInternal();
		}

		public string ReadUrl()
		{
			return _remoteConfigLoader.ReadValue( Key ).StringValue;
		}

		private void InitializeInternal()
		{
			try
			{
				FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith( task =>
				{
					var dependencyStatus = task.Result;
					if ( dependencyStatus == DependencyStatus.Available )
					{
						SetDefaults();
						Debug.Log( "Firebase Initialized" );
					}
					else
					{
						Debug.LogError( $"Could not resolve all Firebase dependencies: {dependencyStatus}" );
					}
				} );
			}
			catch ( Exception ex )
			{
				Debug.LogError( $"Firebase Initialize Exception : {ex.Message}" );
				_onErrorAction?.Invoke( ex.Message );
			}
		}

		private void SetDefaults()
		{
			Dictionary<string, object> defaults = new Dictionary<string, object>();
			defaults.Add( Key, "" );

			_remoteConfigLoader.SetDefault( defaults );
		}
	}
}