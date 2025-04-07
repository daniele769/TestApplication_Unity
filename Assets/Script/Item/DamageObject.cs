using System;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField] 
    private float damage = 30f;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
            return;
        
        if(other.CompareTag("Player"))
            HealthManager.Instance.InvokeDamage(damage);
    }
}
