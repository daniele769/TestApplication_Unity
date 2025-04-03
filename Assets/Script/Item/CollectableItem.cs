using System;
using UnityEngine;

public enum ItemType
{
    Coin = 0,
    Potion = 1,
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

    private void Start()
    {
        _defaultScale = transform.localScale;
    }

    private void Update()
    {
        switch (itemType)
        {
            case ItemType.Coin: _dirToRotate = Vector3.forward; break;
            case ItemType.Potion: _dirToRotate = Vector3.up; break;
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
            }
            
            Destroy(this.gameObject);
        }
    }
}
