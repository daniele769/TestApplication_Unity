using System;
using UnityEngine;

public class CollectableItemManager : MonoBehaviour
{
    public static CollectableItemManager Instance { get; private set;  }

    public event Action OnAddCoin;
    public event Action OnAddPotion;

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
    
    public void InvokeAddCoin()
    {
        OnAddCoin?.Invoke();
    }
    
    public void InvokeAddPotions()
    {
        OnAddPotion?.Invoke();
    }
    
    void Update()
    {
        
    }
}
