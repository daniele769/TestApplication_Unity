using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PillarsPuzzleControl : MonoBehaviour
{
    private int _pillarsInPosition;
    private AudioSource _audioSource;
    private bool _isCompleted;

    public event Action OnComplete;
    
    void Start()
    {
        _pillarsInPosition = 0;
        _audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (_pillarsInPosition == 3)
        {
            if (!_audioSource.isPlaying && !_isCompleted)
            {
                print("Puzzle completed!!!");
                _audioSource.Play();
                _isCompleted = true;
                OnComplete?.Invoke();
            }
        }
    }

    public void AddPillarInPosition()
    {
        _pillarsInPosition++;
    }
    
    public void RemovePillarInPosition()
    {
        _pillarsInPosition--;
    }
}
