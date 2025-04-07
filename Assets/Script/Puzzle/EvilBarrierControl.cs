using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBarrierControl : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration = 2f;

    [SerializeField] 
    private int enemiesToKill = 3;
    
    public event Action OnComplete;

    private AudioSource _audioSource;
    private Material _material;
    private int _puzzleCompleted;
    private bool _isCompleted;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _material = GetComponent<Renderer>().material;
        
        _puzzleCompleted = 0;

        OnComplete += OnCompleteAction;
    }
    
    void Update()
    {
        if (!_isCompleted && _puzzleCompleted == enemiesToKill)
        {
            OnComplete?.Invoke();
        }
    }

    private void OnCompleteAction()
    {
        _isCompleted = false;
        _puzzleCompleted = 0;
        
        _audioSource.Play();
        StartCoroutine(DestroyBarrier());
    }

    private IEnumerator DestroyBarrier()
    {
        Color color = _material.color;
        float startAlpha = color.a;
        float t = 0;
        
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            _material.color = color;
            yield return null;
        }

        color.a = 0f;
        _material.color = color;
        Destroy(this.gameObject);
    }

    public void AddPuzzleCompleted()
    {
        _puzzleCompleted++;
    }
}
