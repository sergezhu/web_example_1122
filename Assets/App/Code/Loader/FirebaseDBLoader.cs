namespace App.Code.Loader
{
	using System;
	using System.Threading.Tasks;
	using Firebase.Database;
	using Firebase.Extensions;
	using UniRx;
	using UnityEngine;

	public interface IFirebaseDBLoader<T>
	{
		void SaveToDB(T data, string path);
		Task<T> LoadFromDB( string path );
	}

	public abstract class FirebaseDBLoader<T> : IFirebaseDBLoader<T> where T : class
	{
		private InternetStateService _internetStateService;
		private DatabaseReference _dbRef;

		protected FirebaseDBLoader( InternetStateService internetStateService )
		{
			_internetStateService = internetStateService;
			_dbRef = FirebaseDatabase.DefaultInstance.RootReference;

			Debug.Log( $"FirebaseDBLoader Initialized" );
		}

		public ReactiveCommand SuccessfullyLoad { get; } = new ReactiveCommand();
		public ReactiveCommand FailLoad { get; } = new ReactiveCommand();

		public void SaveToDB(T data, string path)
		{
			var json = JsonUtility.ToJson( data );
			_dbRef.Child( path ).SetRawJsonValueAsync( json );
		}

		public async Task<T> LoadFromDB( string path )
		{
			_internetStateService.Check();
			
			if ( !_internetStateService.HasInternet.Value )
			{
				Debug.LogError( "You have no connection" );
				FailLoad.Execute();
				return null;
			}

			T loadResult = null;
			Task<DataSnapshot> loadTask = GetSnapshotMultiAttemptsAsync(_dbRef, path, 20, 100 );

			await loadTask;

			if ( loadTask.Result == null )
			{
				Debug.LogError( $"Texture loading have been failed : {loadTask.Exception}" );
				FailLoad.Execute();
			}
			else
			{
				DataSnapshot snapshot = loadTask.Result;
				loadResult = SnapshotToData( snapshot );
			}

			return loadResult;
		}

		private T SnapshotToData( DataSnapshot src )
		{
			var json = src.GetRawJsonValue();
			var v = JsonUtility.FromJson<T>( json );

			return v;
		}

		private async Task<DataSnapshot> GetSnapshotMultiAttemptsAsync( DatabaseReference dbRef, string path, int attempts, int delayBetweenAttemptsMilliseconds )
		{
			Task<DataSnapshot> downloadTask = dbRef.Child( path ).GetValueAsync();
			DataSnapshot result = null;

			while ( attempts > 0 && result == null )
			{
				await downloadTask.ContinueWithOnMainThread( task =>
				{
					if ( downloadTask.IsFaulted || downloadTask.IsCanceled )
					{
						Debug.LogWarning( $"Requested data from DB loading attempt : FAILED  ( attempts remaining : {attempts} )" );
					}
					else
					{
						result = downloadTask.Result;
						Debug.LogWarning( $"Requested data from DB loading attempt : SUCCESS  ( attempts remaining : {attempts} )" );
					}
				} );

				attempts--;

				await Task.Delay( TimeSpan.FromMilliseconds( delayBetweenAttemptsMilliseconds ) );
			}

			return result;
		}
	}
}