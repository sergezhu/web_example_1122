using UnityEngine;

public class Kegl : MonoBehaviour
{
    [SerializeField] private Transform _centerOfMass;
    [SerializeField] private Rigidbody _rb;
    
    void Awake()
    {
        //_rb.centerOfMass = _centerOfMass.position;
    }
}
