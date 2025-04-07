using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField] 
    private AudioClip damageAudio;
    
    [SerializeField] 
    private AudioClip recoverAudio;
    
    private AudioSource _audioSource;
    private ProgressBar _healtBar;
    private float _healthValue;
    
    void Awake()
    {
        _healtBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>("HealtBar");
        _healthValue = 100;
        _healtBar.value = _healthValue;

        _audioSource = GetComponent<AudioSource>();
    }

    public void Damage(float damage)
    {
        _audioSource.Stop();
        _audioSource.resource = damageAudio;
        _audioSource.Play();
        
        _healthValue -= damage;
        _healtBar.value = _healthValue;
        
        if (_healthValue <= 0)
        {
            HealthManager.Instance.InvokeDeath();
        }
    }

    public void Recover(float value)
    {
        // if (_healthValue == 100f)
        // {
        //     print("Healt already full");
        //     return;
        // }
        
        _audioSource.Stop();
        _audioSource.resource = recoverAudio;
        _audioSource.Play();
        
        _healthValue += value;
        if (_healthValue > 100)
            _healthValue = 100;

        _healtBar.value = _healthValue;
        return;
    }

    public float GetValue()
    {
        return _healthValue;
    }
    
    void Update()
    {
        
    }
}
