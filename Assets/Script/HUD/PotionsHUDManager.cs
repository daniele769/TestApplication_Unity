using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PotionsHUDManager : MonoBehaviour
{
    private int _potionsCount;
    private Label _counterLabel;

    private void Awake()
    {
        _potionsCount = 0;
        _counterLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>("Counter");
        _counterLabel.text = "" + _potionsCount;
    }

    public bool ConsumePotion()
    {
        if (_potionsCount > 0)
        {
            _potionsCount--;
            _counterLabel.text = "" + _potionsCount;
            return true;
        }

        return false;
    }

    public void AddPotions()
    {
        _potionsCount++;
        _counterLabel.text = "" + _potionsCount;
    }


    void Update()
    {
        
    }
}
