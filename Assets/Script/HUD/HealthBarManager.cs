using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarManager : MonoBehaviour
{
    private ProgressBar _healtBar;
    private float _healthValue; 
    
    void Awake()
    {
        _healtBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>("HealtBar");
        _healthValue = 100;
        _healtBar.value = _healthValue;
    }

    public void Damage(float damage)
    {
        _healthValue -= damage;
        _healtBar.value = _healthValue;
        
        if (_healthValue <= 0)
        {
            //TODO Death event
            print("*** Player is death ***");
        }
    }

    public void Recover(float value)
    {
        if (_healthValue == 100f)
        {
            print("Healt already full");
            return;
        }
        
        _healthValue += value;
        if (_healthValue > 100)
            _healthValue = 100;

        _healtBar.value = _healthValue;
        return;
    }
    
    void Update()
    {
        
    }
}
