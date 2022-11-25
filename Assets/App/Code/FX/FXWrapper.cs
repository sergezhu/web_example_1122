namespace App.Code.FX
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	public class FXWrapper : MonoBehaviour
	{
		[SerializeField] private ParticleSystem PS;

		private Transform _fxTransform;
		private Transform _wrapperTransform;
		private ParticleSystemRenderer _psRenderer;
		private ParticleSystem.MainModule _psMain;
		private ParticleSystem.EmissionModule _psEmission;
		private bool _isCached;

		private void Awake()
		{
			CacheValues();
		}

		private void Start()
		{
			ScaleFX();
		}

		private void ScaleFX()
		{
			var scale = Mathf.Sqrt(Screen.height / 1333f);
			
			Debug.Log( $"scaler : {scale}" );

			var currentStartSize = _psMain.startSize;
			_psMain.startSize = new ParticleSystem.MinMaxCurve( currentStartSize.constantMax * scale, currentStartSize.constantMax * scale );

			var currentStartSpeed = _psMain.startSpeed;
			_psMain.startSpeed = new ParticleSystem.MinMaxCurve( currentStartSpeed.constantMin * scale, currentStartSpeed.constantMax * scale );
		}


		public void Play()
		{
			if ( !PS.isPlaying )
				PS.Play( true );
		}

		public void PlayProperly( bool withChildren = true )
		{
			if ( PS.isPlaying )
				PS.Stop( withChildren, ParticleSystemStopBehavior.StopEmittingAndClear );

			PS.Play( withChildren );
		}

		public void Stop()
		{
			if ( PS.isPlaying )
				PS.Stop( true );
		}

		public void Pause()
		{
			if ( !PS.isPaused )
				PS.Pause(true);
		}

		public void SetPosition( Vector3 position )
		{
			_wrapperTransform.position = position;
		}

		public void SetRotation( Quaternion rotation )
		{
			_wrapperTransform.rotation = rotation;
		}

		public void Show()
		{
			_fxTransform.gameObject.SetActive( true );
		}

		public void Hide()
		{
			_fxTransform.gameObject.SetActive( false );
		}

		public void SetEmission(float value)
		{
			_psEmission.rateOverTime = value;
		}

		public void CacheValues()
		{
			if ( _isCached )
				return;

			CacheValuesInternal();

			_isCached = true;
		}

		protected virtual void CacheValuesInternal()
		{
			_wrapperTransform = transform;
			_fxTransform = PS.transform;
			_psMain = PS.main;
			_psEmission = PS.emission;
			_psRenderer = PS.GetComponent<ParticleSystemRenderer>();
		}
	}
}

