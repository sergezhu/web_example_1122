using UnityEngine;

namespace App.Code.Game
{
    using DG.Tweening;
    using UnityEngine.UI;

    public class Pic : MonoBehaviour
    {
        [SerializeField] private int _id;
        
        [Space]
        [SerializeField] private Image _normal;
        [SerializeField] private Image _blurred;
        [SerializeField] private RawImage _winFX;
        [SerializeField] private Image _mark;
        
        private bool _isStop;
        private Material _material1;
        private Material _material2;
        private float _speedThreshold;

        private Tween[] _tweens;

        public int ID => _id;

        private void Awake()
        {
            _normal.material = new Material( _normal.material );
            _blurred.material = new Material( _blurred.material );

            _tweens = new Tween[3];
            
            SetWinFXState( false );
            SetMarkState( false );
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

        public void SetWinFXState( bool state )
        {
            _winFX.gameObject.SetActive( state );
        }

        public void SetMarkState( bool state )
        {
            _mark.gameObject.SetActive( state );
        }

        private void SetBlurActiveState( float speed )
        {
            //Debug.Log( $"SetBlurActiveState {name}, threshol  {_speedThreshold}" );
            
            var needBlur = speed >= _speedThreshold;
            var isBlurred = _blurred.gameObject.activeSelf;
            var transitionDuration = 2f * Mathf.Min(speed, 2f);
            
            if ( isBlurred == needBlur )
                return;

            if ( needBlur )
            {
                ClearTweens();
                
                _blurred.gameObject.SetActive( true );
                _tweens[0] = _blurred.DOFade( 1, transitionDuration );
                _tweens[1] = _normal.DOFade( 0, transitionDuration ).OnComplete( () => _normal.gameObject.SetActive( false ) );
            }
            else
            {
                ClearTweens();
                
                _normal.gameObject.SetActive( true );
                _tweens[0] = _normal.DOFade( 1, transitionDuration );
                _tweens[1] = _blurred.DOFade( 0, transitionDuration ).OnComplete( () => _blurred.gameObject.SetActive( false ) );
            }
        }

        private void ClearTweens()
        {
            for ( var i = 0; i < _tweens.Length; i++ )
            {
                _tweens[i]?.Kill();
                _tweens[i] = null;
            }
        }
    }
}
