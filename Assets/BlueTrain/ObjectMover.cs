using UnityEngine;

public sealed class ObjectMover : MonoBehaviour
{
    public static ObjectMover instance;

    [SerializeField] private float speed = 5f;

    private Transform _tr;
    
    
    private bool _moving;

    private void Awake()
    {
        instance = this;
        _tr = transform;
    }

    
}