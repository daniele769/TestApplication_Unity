using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip waterFootstep;
    
    [SerializeField]
    private AudioClip waterSplash;
    
    [SerializeField]
    private AudioClip waterSwim;
    
    [SerializeField]
    private float _pitchRandomInterval = 0.2f;
    
    private PlayerControllerRigid _playerController;
    private Transform _swimPos;
    private AudioSource _audioSourceMovement;
    private AudioSource _audioSourceSplash;
    
    
    private void Awake()
    {
        _audioSourceMovement = GetComponents<AudioSource>()[0];
        _audioSourceSplash = GetComponents<AudioSource>()[1];
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
            _playerController.SetIsInsideWater(true);

            if (!_playerController.CheckGround())
            {
                print("Splash");
                _audioSourceSplash.resource = waterSplash;
                _audioSourceSplash.Play();
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_playerController)
        {
            //PlayerControllerRigid playerController = other.GetComponent<PlayerControllerRigid>();
            if (_swimPos.position.y <= transform.position.y)
            {
                _playerController.SetIsSwimming(true);   
            }
            else
            {
                _playerController.SetIsSwimming(false); 
            }

            //PlayAudio
            if (_playerController.IsMoving())
            {
                if (_playerController.IsSwimming())
                {
                    if (_audioSourceMovement.resource != waterSwim)
                        _audioSourceMovement.resource = waterSwim;

                    if (!_audioSourceMovement.isPlaying)
                    {
                        float pitchOffset = Random.Range(-1 * _pitchRandomInterval, _pitchRandomInterval);
                        _audioSourceMovement.pitch = 1 + pitchOffset;
                        _audioSourceMovement.PlayDelayed(0.3f);
                    }
                    return;
                }

                if (_playerController.IsRunning())
                {
                    if (_audioSourceMovement.resource != waterFootstep)
                        _audioSourceMovement.resource = waterFootstep;
                
                    if (!_audioSourceMovement.isPlaying && _playerController.CheckGround())
                    {
                        float pitchOffset = Random.Range(-1 * _pitchRandomInterval, _pitchRandomInterval);
                        _audioSourceMovement.pitch = 1 + pitchOffset;
                        _audioSourceMovement.Play();
                    }
                    return;
                }
                
                if (_audioSourceMovement.resource != waterFootstep)
                    _audioSourceMovement.resource = waterFootstep;
                
                if (!_audioSourceMovement.isPlaying)
                {
                    float pitchOffset = Random.Range(-1 * _pitchRandomInterval, _pitchRandomInterval);
                    _audioSourceMovement.pitch = 1 + pitchOffset;
                    _audioSourceMovement.PlayDelayed(0.3f);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player outside water");
            _playerController.AdjustCameraBeforeSwimming(false);
            _playerController.SetIsSwimming(false);
            _playerController.SetIsInsideWater(false);
            _playerController = null;
            _swimPos = null;
        }
    }
}
