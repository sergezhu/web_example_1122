using System;
using UnityEngine;

namespace App.Code.Game
{
    using DG.Tweening;
    using UnityEngine.UI;

    public class Pic : MonoBehaviour
    {
        [SerializeField] private Image _normal;
        [SerializeField] private Image _blurred;
        
        private bool _isStop;
        private Material _material1;
        private Material _material2;
        private float _speedThreshold;

        private void Awake()
        {
            _normal.material = new Material( _normal.material );
            _blurred.material = new Material( _blurred.material );
        }

        public void Construct( float speedThreshold )
        {
            _speedThreshold = speedThreshold;
            SetBlurActiveState( 0 );
        }

        public void UpdateView( float speed )
        {
            speed = Mathf.Max( speed, 0 );
            _isStop = speed < float.Epsilon;

            if ( !_isStop ) 
                SetBlurActiveState( speed );
        }

        private void SetBlurActiveState( float speed )
        {
            Debug.Log( $"SetBlurActiveState {name}, threshol  {_speedThreshold}" );
            
            var needBlur = speed >= _speedThreshold;
            var isBlurred = _blurred.gameObject.activeSelf;
            var transitionDuration = 2f * Mathf.Min(speed, 2f);
            
            if ( isBlurred == needBlur )
                return;

            if ( needBlur )
            {
                _blurred.gameObject.SetActive( true );
                _blurred.DOFade( 1, transitionDuration );
                _normal.DOFade( 0, transitionDuration ).SetDelay( transitionDuration ).OnComplete( () => _normal.gameObject.SetActive( false ) );
            }
            else
            {
                _normal.gameObject.SetActive( true );
                _normal.DOFade( 1, transitionDuration );
                _blurred.DOFade( 0, transitionDuration ).SetDelay( transitionDuration ).OnComplete( () => _blurred.gameObject.SetActive( false ) );
            }
        }

        /*private void SetBlurredState( float speed )
        {
            var blurredWeight = Mathf.InverseLerp( Speed1, Speed2, speed );
            
            //Debug.Log( $"{speed} => {blurredWeight}" );

            var color1 = _normal.material.color;
            color1.a = 1 - blurredWeight;
            _normal.material.color = color1;

            var color2 = _blurred.material.color;
            color2.a = blurredWeight;
            _blurred.material.color = color2;
        }*/
    }
}
