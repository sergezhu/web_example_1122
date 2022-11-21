using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class AspectRatioConstraint : MonoBehaviour
{
    [SerializeField] private Vector2 _minMaxRatio;
    [SerializeField] private float w;
    [SerializeField] private float h;
    [SerializeField] private float x;
    [SerializeField] private float y;
    [SerializeField, HideInInspector] private RectTransform _rectTransform;
    
    
    [ExecuteAlways]
    private void Update()
    {
        var rect = _rectTransform.rect;
        /*h = rect.height;
        w = rect.width;
        var ratio = h / w;

        var newRatio = ratio;
        
        if ( ratio > _minMaxRatio.y )
        {
            newRatio = _minMaxRatio.y;
            w = h / ratio;
        }
        else if ( ratio < _minMaxRatio.x )
        {
            newRatio = _minMaxRatio.x;
            h = w * ratio;
        }*/

        _rectTransform.sizeDelta = new Vector2( w, h );
        x = rect.x;
        y = rect.y;
    }

    private void OnValidate()
    {
        _rectTransform ??= GetComponent<RectTransform>();
        
        if ( _minMaxRatio.x > _minMaxRatio.y )
            _minMaxRatio = new Vector2( _minMaxRatio.y, _minMaxRatio.x );
    }
}
