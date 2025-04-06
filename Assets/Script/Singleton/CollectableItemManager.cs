using System;
using UnityEngine;

public class CollectableItemManager : MonoBehaviour
{
    public static CollectableItemManager Instance { get; private set;  }

    private bool _haveShield;
    private bool _haveSword;
    public event Action OnAddCoin;
    public event Action OnAddPotion;
    public event Action OnAddSword;
    public event Action OnAddShield;

    [SerializeField]
    private PotionsHUDManager potionsHUD;
    
    [SerializeField]
    private CoinsHUDManager coinsHUD;
    
    private void Awake()
    {
        //this is a singleton, only one instance have to exist
        if (Instance == null)
        {
            Instance = this;
            OnAddCoin += AddCoin;
            OnAddPotion += AddPotion;
            OnAddShield += AddShield;
            OnAddSword += AddSword;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void AddCoin()
    {
        coinsHUD.AddCoins();
    }
    
    private void AddPotion()
    {
        potionsHUD.AddPotions();
    }

    private void AddShield()
    {
        _haveShield = true;
    }
    
    private void AddSword()
    {
        _haveSword = true;
    }
    
    public void InvokeAddCoin()
    {
        OnAddCoin?.Invoke();
    }
    
    public void InvokeAddPotions()
    {
        OnAddPotion?.Invoke();
    }

    public void InvokeAddSword()
    {
        OnAddSword?.Invoke();
    }

    public void InvokeAddShield()
    {
        OnAddShield?.Invoke();
    }

    public bool HaveShield()
    {
        return _haveShield;
    }
    
    public bool HaveSword()
    {
        return _haveSword;
    }

    public void RemoveSword()
    {
        _haveSword = false;
    }
    
    public void RemoveShield()
    {
        _haveShield = false;
    }
    
    void Update()
    {
        
    }
}
