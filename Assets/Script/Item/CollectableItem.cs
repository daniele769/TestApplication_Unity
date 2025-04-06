using System;
using UnityEngine;

public enum ItemType
{
    Coin = 0,
    Potion = 1,
    HolyShield = 2,
    HolySword = 3
}

public class CollectableItem : MonoBehaviour
{
    [SerializeField]
    private float speedRot = 90f;
    
    [SerializeField]
    private float pulseSpeed = 2.5f;
    
    [SerializeField]
    private float pulseMagnitude = 0.15f;
    
    public ItemType itemType;

    private Vector3 _dirToRotate;
    private Vector3 _defaultScale;
    private AudioSource _audioSource;
    private bool _haveToDestroy;

    private void Start()
    {
        _defaultScale = transform.localScale;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_haveToDestroy & !_audioSource.isPlaying)
            Destroy(this.gameObject);
        
        switch (itemType)
        {
            case ItemType.Coin: _dirToRotate = Vector3.forward; break;
            case ItemType.Potion: _dirToRotate = Vector3.up; break;
            case ItemType.HolyShield: _dirToRotate = Vector3.up; break;
            case ItemType.HolySword: _dirToRotate = Vector3.up; break;
        }
        transform.Rotate(_dirToRotate, speedRot * Time.deltaTime);
        float pulseFactor = (float) (1 + Math.Sin(Time.time * pulseSpeed) * pulseMagnitude);
        transform.localScale = _defaultScale * pulseFactor;

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player collect item: " + this.name);
            
            switch (itemType)
            {
                case ItemType.Coin: CollectableItemManager.Instance.InvokeAddCoin(); break;
                case ItemType.Potion: CollectableItemManager.Instance.InvokeAddPotions(); break;
                case ItemType.HolyShield: CollectableItemManager.Instance.InvokeAddShield(); break;
                case ItemType.HolySword: CollectableItemManager.Instance.InvokeAddSword(); break;
            }
            
            _audioSource.Play();
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            GetComponent<SphereCollider>().enabled = false;
            _haveToDestroy = true;
        }
    }
}
