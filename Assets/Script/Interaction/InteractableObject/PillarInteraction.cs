using System.Collections.Generic;
using UnityEngine;

public class PillarInteraction : AbstractInteractableObject
{
    [SerializeField] 
    private List<Transform> grabPoints;
    
    [SerializeField]
    private PillarsPuzzleControl pillarsPuzzle;
    
    [SerializeField]
    private GameObject fireParticle;
    
    private Light _pointLight;
    
    private AudioSource _audioSource;
    private bool _isGrabbing;
    private FixedJoint _fixedJoint;
    private Collider _objectCollider;
    private Collider _playerCollider;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _objectCollider = GetComponent<Collider>();
        _pointLight = GetComponentInChildren<Light>();
        
        pillarsPuzzle.OnComplete += () => { _pointLight.enabled = true; fireParticle.SetActive(true);};
    }
    
    void Update()
    {
        
    }

    public override void Interact(Transform player)
    {
        if (!_isGrabbing)
        {
            GrabInteraction(player);
            return;
        }
        ReleaseInteraction();
    }

    private void GrabInteraction(Transform player)
    {
        _isGrabbing = true;
        popupInteraction.SetText("Release");
        
        MovePlayerToGrabPoint(player);
        
        _playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(_playerCollider, _objectCollider, true);
        _fixedJoint = player.GetComponentInChildren<FixedJoint>();
        _fixedJoint.connectedBody = GetComponent<Rigidbody>();
        
        player.GetComponent<Animator>().SetTrigger(Constants.PushTrigger);
        player.GetComponent<PlayerControllerRigid>().SetIsGrabbing(true);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ReleaseInteraction()
    {
        _isGrabbing = false;
        popupInteraction.SetText("Grab");
        
        _playerCollider.GetComponent<Animator>().SetTrigger(Constants.ReleaseTrigger);
        _playerCollider.GetComponent<PlayerControllerRigid>().SetIsGrabbing(false);
        
        Physics.IgnoreCollision(_playerCollider, _objectCollider, false);
        _fixedJoint.connectedBody = null;
        _fixedJoint = null;
        _playerCollider = null;
        
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void MovePlayerToGrabPoint(Transform player)
    {
        Vector3 grabPosition = grabPoints[0].position;
        int pos = 0;
        
        float distance = Vector3.Distance(player.position, grabPoints[0].position);
        foreach (Transform grabPoint in  grabPoints)
        {
            if (Vector3.Distance(player.position, grabPoint.position) < distance)
            {
                distance = Vector3.Distance(player.position, grabPoint.position);
                grabPosition = grabPoint.position;
                pos = grabPoints.IndexOf(grabPoint);
            }
        }
        
        Vector3 offset = grabPosition - player.position;
        player.position = new Vector3(player.position.x + offset.x, player.position.y, player.position.z + offset.z);
        player.LookAt(transform);
    }
}
