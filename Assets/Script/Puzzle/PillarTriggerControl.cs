using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PillarTriggerControl : MonoBehaviour
{
    private PillarsPuzzleControl _pillarsPuzzle;
    
    //private AudioSource _audioSource;
    
    void Start()
    {
        _pillarsPuzzle = GetComponentInParent<PillarsPuzzleControl>();
        //_audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MovablePillar"))
        {
            print("Pillar entered");
            _pillarsPuzzle.AddPillarInPosition();
            // if(!_audioSource.isPlaying) 
            //     _audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MovablePillar"))
        {
            print("Pillar exited");
            _pillarsPuzzle.RemovePillarInPosition();
        }
    }
}
