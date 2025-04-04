using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class CoinsHUDManager : MonoBehaviour
{
    [HideInInspector] 
    public int coinsCount;
    
    private Label _counterLabel;

    private void Awake()
    {
        coinsCount = 0;
        _counterLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Counter");
        _counterLabel.text = "" + coinsCount;
        
    }

    public void ConsumeCoin(int val)
    {
        if (coinsCount > 0)
        {
            coinsCount -= val;
            _counterLabel.text = "" + coinsCount;
        }
    }

    public void AddCoins()
    {
        coinsCount++;
        _counterLabel.text = "" + coinsCount;
    }


    void Update()
    {
        
    }
}
