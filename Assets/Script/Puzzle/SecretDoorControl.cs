using System.Collections;
using UnityEngine;

public class SecretDoorControl : MonoBehaviour
{
    [SerializeField] 
    private PillarsPuzzleControl puzzleControl;

    [SerializeField] 
    private float offset = 6.05f;
    
    [SerializeField] 
    private float speed = 1f;

    private float _distance = 0;
    private AudioSource _audioSource;
    
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        puzzleControl.OnComplete += OpenDoor;
    }
    
    void Update()
    {
    }

    private void OpenDoor()
    {
        StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        _audioSource.Play();
        while(_distance < offset)
        {
            _distance += speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        _audioSource.Stop();
    }
}
