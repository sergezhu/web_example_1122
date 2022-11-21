using System;
using UnityEngine;

namespace App.Code.Game
{
    using UnityEngine.UI;

    public class Pic : MonoBehaviour
    {
        private const float Speed1 = 10;
        private const float Speed2 = 15;
    
        [SerializeField] private Image _normal;
        [SerializeField] private Image _blurred;
        
        private bool _isStop;
        private Material _material1;
        private Material _material2;

        private void Awake()
        {
            SetActiveState( true );
            SetBlurredState( 0 );

            _normal.material = new Material( _normal.material );
            _blurred.material = new Material( _blurred.material );
        }

        public void UpdateView( float speed )
        {
            speed = MathF.Max( speed, 0 );
            var isStop = speed < float.Epsilon;

            if ( isStop != _isStop )
            {
                _isStop = isStop;
                SetActiveState( _isStop );
            }

            if ( !_isStop )
            {
                SetBlurredState( speed );
            }
        }

        private void SetActiveState( bool isStop )
        {
            _normal.gameObject.SetActive( isStop );
            _blurred.gameObject.SetActive( !isStop );
        }

        private void SetBlurredState( float speed )
        {
            var blurredWeight = Mathf.InverseLerp( Speed1, Speed2, speed );

            var color1 = _normal.material.color;
            color1.a = 1 - blurredWeight;
            _normal.material.color = color1;

            var color2 = _normal.material.color;
            color2.a = blurredWeight;
            _normal.material.color = color2;
        }
    }
}
