using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

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
    
    [SerializeField] 
    private UIDocument redVignette;

    private VisualElement _redVignetteElement;
    
    private void Awake()
    {
        //this is a singleton, only one instance have to exist
        if (Instance == null)
        {
            Instance = this;
            _redVignetteElement = redVignette.rootVisualElement.Q<VisualElement>("RedVignette");
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
        if (healthBar.GetValue() < 50)
        {
            float alpha = Mathf.Clamp01(1 - healthBar.GetValue() / 50);
            Color color = _redVignetteElement.style.unityBackgroundImageTintColor.value;
            color.r = 255;
            color.g = 0;
            color.b = 0;
            color.a = alpha * 255;
            print("Background alpha is " + color.a);
            _redVignetteElement.style.unityBackgroundImageTintColor = new StyleColor(color);
        }
        else if (_redVignetteElement.style.unityBackgroundImageTintColor.value.a != 0)
        {
            Color color = _redVignetteElement.style.unityBackgroundImageTintColor.value; 
            color.a = 0; 
            _redVignetteElement.style.unityBackgroundImageTintColor = new StyleColor(color);
            
        }
    }
}
