using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    public Action OnDamage;
    public Action OnRecover;
    public event Action OnDeath;

    private float _damageValue;
    private bool _isDeath;
    
    [SerializeField]
    private float recoverValue = 50;

    [SerializeField] 
    private HealthBarManager healthBar;
    
    [SerializeField] 
    private PotionsHUDManager potionsHUD;
    
    private void Awake()
    {
        //this is a singleton, only one instance have to exist
        if (Instance == null)
        {
            Instance = this;
            _damageValue = 0;
            OnDamage += MakeDamage;
            OnRecover += Recover;
            OnDeath += () => { _isDeath = true; };
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void MakeDamage()
    {
        if(!_isDeath)
            healthBar.Damage(_damageValue);
    }
    
    private void Recover()
    {
        if (potionsHUD.ConsumePotion())
        {
            healthBar.Recover(recoverValue);
            return;
        }
        
        print("No Potions avaiable");
    }
    
    public void InvokeDamage(float damage)
    {
        _damageValue = damage;
        OnDamage?.Invoke();
    }

    public void InvokeRecover()
    {
        OnRecover?.Invoke();
    }

    public void InvokeDeath()
    {
        print("*** Player is death ***");
        OnDeath?.Invoke();
    }


    void Update()
    {
        
    }
}
