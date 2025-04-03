using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinsHUDManager : MonoBehaviour
{
    private int _potionsCount;
    private Label _counterLabel;

    private void Awake()
    {
        _potionsCount = 0;
        _counterLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Counter");
        _counterLabel.text = "" + _potionsCount;
        
    }

    private void ConsumeCoin()
    {
        if (_potionsCount > 0)
        {
            _potionsCount--;
            _counterLabel.text = "" + _potionsCount;
        }
    }

    public void AddCoins()
    {
        _potionsCount++;
        _counterLabel.text = "" + _potionsCount;
    }


    void Update()
    {
        
    }
}
