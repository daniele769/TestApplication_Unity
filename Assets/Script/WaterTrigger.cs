using System;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    private PlayerControllerRigid _playerController;
    private Transform _swimPos;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player inside water");
            _swimPos = other.transform.Find(Constants.SwimPos);
            _playerController = other.GetComponent<PlayerControllerRigid>();
            _playerController.AdjustCameraBeforeSwimming(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_playerController)
        {
            if (_swimPos.position.y <= transform.position.y)
            {
                
                _playerController.IsSwimming(true);   
            }
            else
            {
                _playerController.IsSwimming(false); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player outside water");
            _playerController.AdjustCameraBeforeSwimming(false);
            _playerController.IsSwimming(false);
            _playerController = null;
            _swimPos = null;
        }
    }
}
