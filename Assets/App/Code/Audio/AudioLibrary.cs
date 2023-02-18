namespace App.Code.Audio
{
	using UnityEngine;
	using UnityEngine.Serialization;

	public class AudioLibrary : MonoBehaviour
	{
		public AudioSource BgMusic;
		public AudioSource Start;
		[FormerlySerializedAs( "RightAnswer" )] public AudioSource WinResult;
		[FormerlySerializedAs( "FailAnswer" )] public AudioSource LoseResult;
		public AudioSource Click;
		public AudioSource Hit;
		public AudioSource Tic;
		public AudioSource Toc;
	}
}