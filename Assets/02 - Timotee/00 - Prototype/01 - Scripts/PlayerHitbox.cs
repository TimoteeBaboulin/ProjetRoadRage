using System;
using UnityEngine;

public enum HitboxType
{
    Front = 0,
    Side = 1
}
public class PlayerHitbox : MonoBehaviour
{
    public static event Action<HitboxType> OnContact; 
    
    [SerializeField] private HitboxType _hitboxType;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Obstacle")) return;
        
        OnContact?.Invoke(_hitboxType);
    }
}