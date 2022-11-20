namespace App.Code
{
	using UnityEngine;

	public class PlayerPrefsSystem
	{
		
		private const string PreferencesKey = "Preferences";

		public PreferencesData Load()
		{
			PreferencesData loadData;
			var exist = PlayerPrefs.HasKey( PreferencesKey );

			Debug.Log( $"PlayerPrefs is exist : {exist}" );
			
			if ( exist )
			{
				var json = PlayerPrefs.GetString( PreferencesKey );
				loadData = JsonUtility.FromJson<PreferencesData>( json );

				return loadData;
			}

			return null;
		}


		public void Save( PreferencesData data )
		{
			var json = JsonUtility.ToJson( data );
			PlayerPrefs.SetString( PreferencesKey, json );
		}


		#if UNITY_EDITOR
		public void DeletePlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}
		#endif
	}
}

