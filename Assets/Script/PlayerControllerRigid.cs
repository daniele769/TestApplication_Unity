using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum GroundType
{
    Stone = 0,
    Wood = 1,
    Grass = 2
}
public class PlayerControllerRigid : MonoBehaviour
{
    private Rigidbody _body;
    private bool _isMoving;
    private bool _isRunning;
    private bool _isSwimming;
    private bool _isInsideWater;
    private Vector2 _inputVector;
    private Vector3 _terrainNormal;
    private Animator _animator;
    private CapsuleCollider _capsuleCollider;
    private PlayerInput _playerInput;
    private float _defaultBottomHeightCinemachine;
    private Coroutine _lerpCameraCoroutine;
    private AudioSource _audioSource;
    private bool _isFootstep1;
    private float _walkDelaySound;
    private GroundType _groundType;
    
    [SerializeField]
    private float walkSpeed = 5f;

    [SerializeField] 
    private float runSpeed = 10f;

    [SerializeField] 
    private float jumpHeight = 8f;
    
    [SerializeField] 
    private float rotSpeed = 7f;

    [SerializeField] 
    private float fallingOffset = 0.1f;
    
    [SerializeField] 
    private float hStairsOff = 0.3f;
    
    [SerializeField] 
    private float vStairsOff = 0.4f;
    
    [SerializeField] 
    private float stairsUpSpeed = 1f;
    
    [SerializeField] 
    private float lerpCameraSpeed = 2f;

    [SerializeField] 
    private Camera myCamera;
    
    [SerializeField] 
    private CinemachineOrbitalFollow cinemachineCamera;

    [SerializeField]
    private AudioClip grassFootstep1;
    
    [SerializeField]
    private AudioClip grassFootstep2;
    
    [SerializeField]
    private AudioClip stoneFootstep1;
    
    [SerializeField]
    private AudioClip stoneFootstep2;
    
    [SerializeField]
    private AudioClip woodFootstep1;
    
    [SerializeField]
    private AudioClip woodFootstep2;
    
    [SerializeField]
    private float _pitchRandomInterval = 0.2f;
    
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _playerInput = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();

        _isFootstep1 = true;
        _terrainNormal = Vector3.up;
        PlayerInputState.CurrentControlScheme = _playerInput.currentControlScheme;
        PlayerInputState.CurrentActionMap = _playerInput.currentActionMap.name;
        _defaultBottomHeightCinemachine = cinemachineCamera.Orbits.Bottom.Height;
    }
    
    void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        PlayerInputState.CurrentControlScheme = _playerInput.currentControlScheme;
        PlayerInputState.CurrentActionMap = _playerInput.currentActionMap.name;
        CheckStairs();
        CheckFalling();
        PlayFootstep();
    }

    private void PlayFootstep()
    {
        if (CheckGround() && !_isInsideWater)
        {
            if (IsMoving())
            {
                if (_isRunning)
                    _walkDelaySound = 0;
                else
                    _walkDelaySound = 0.3f;
                
                if (!_audioSource.isPlaying)
                {
                    if (_groundType == GroundType.Grass)
                    {
                        if (_isFootstep1)
                        {
                            if (_audioSource.resource != grassFootstep1)
                                _audioSource.resource = grassFootstep1;
                        }
                        else 
                        if (_audioSource.resource != grassFootstep2)
                            _audioSource.resource = grassFootstep2;
                    }
                    else if (_groundType == GroundType.Stone)
                    {
                        if (_isFootstep1)
                        {
                            if (_audioSource.resource != stoneFootstep1)
                                _audioSource.resource = stoneFootstep1;
                        }
                        else 
                        if (_audioSource.resource != stoneFootstep2)
                            _audioSource.resource = stoneFootstep2;
                    }
                    else
                    {
                        if (_isFootstep1)
                        {
                            if (_audioSource.resource != woodFootstep1)
                                _audioSource.resource = woodFootstep1;
                        }
                        else 
                        if (_audioSource.resource != woodFootstep2)
                            _audioSource.resource = woodFootstep2;
                    }
                    
                    print("PlayAudio");
                    float pitchOffset = Random.Range(-1 * _pitchRandomInterval, _pitchRandomInterval);
                    _audioSource.pitch = 1 + pitchOffset;
                    _audioSource.PlayDelayed(_walkDelaySound);
                    _isFootstep1 = !_isFootstep1;
                }
                
            }
        }
    }

    public void SetIsSwimming(bool val)
    {
        if (_isSwimming == val)
            return;
        
        _isSwimming = val;
        _animator.SetBool(Constants.IsSwimming, val);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = !val;

        if (_isSwimming)
        {
            rigidbody.linearVelocity = Vector3.zero;
            _isRunning = false;
            _animator.SetBool(Constants.IsRunning, _isRunning);
            
        }
    }

    public void AdjustCameraBeforeSwimming(bool isInsideWater)
    {
        //Change camera orbit to prevent see inside water
        if (isInsideWater)
        {
            if (_lerpCameraCoroutine != null)
            {
                StopCoroutine(_lerpCameraCoroutine);
                _lerpCameraCoroutine = null;
            }
            
            cinemachineCamera.Orbits.Bottom.Height = 2.1f;
        }
        else
        {
            _lerpCameraCoroutine = StartCoroutine(LerpCamera());
        }
    }

    private IEnumerator LerpCamera()
    {
        if (cinemachineCamera.Orbits.Bottom.Height > _defaultBottomHeightCinemachine)
        {
            print("Lerping camera");
            cinemachineCamera.Orbits.Bottom.Height -= lerpCameraSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            _lerpCameraCoroutine = StartCoroutine(LerpCamera());
        }
        else
        {
            cinemachineCamera.Orbits.Bottom.Height = _defaultBottomHeightCinemachine;
            _lerpCameraCoroutine = null;
            yield return 0;
        }
        
    }

    private void MovePlayer()
    {
        if (_inputVector != Vector2.zero)
        {
            _isMoving = true;
            _animator.SetBool(Constants.IsMoving, true);
            Vector3 hVector = myCamera.transform.right;
            Vector3 vVector = myCamera.transform.forward;
            hVector.y = 0;
            vVector.y = 0;
        
            Vector3 moveVector = hVector * _inputVector.x + vVector * _inputVector.y;
            moveVector = moveVector.normalized;
            UpdatePlayerRotation(moveVector);
            
            if(_isSwimming)
                _terrainNormal = Vector3.up;
            
            Vector3 moveVectorOnGround = Vector3.ProjectOnPlane(moveVector, _terrainNormal).normalized;
            if(_isRunning)
                _body.MovePosition(_body.position + moveVectorOnGround * (Time.fixedDeltaTime * runSpeed));
            else
                _body.MovePosition(_body.position + moveVectorOnGround * (Time.fixedDeltaTime * walkSpeed));
            
            return;
        }
        _animator.SetBool(Constants.IsMoving, false);
        _animator.SetBool(Constants.IsRunning, false);
        _isRunning = false;
        _isMoving = false;
    }
    
    private void UpdatePlayerRotation(Vector3 moveVector)
    {
        if (moveVector != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVector), rotSpeed * Time.deltaTime);
        }
    }

    public bool CheckGround()
    {
        bool isGrouded;
        RaycastHit hit;
        
        Vector3 pos = transform.position;
        pos.y += 0.25f;
        if(Physics.SphereCast(pos, 0.2f, Vector3.down, out hit, 0.5f + fallingOffset))
        {
            isGrouded = true;
            _terrainNormal = hit.normal;
            if (hit.transform.CompareTag("Stone"))
            {
                _groundType = GroundType.Stone;
            }
            else if (hit.transform.CompareTag("Wood"))
            {
                _groundType = GroundType.Wood;
            }
            else
                _groundType = GroundType.Grass;
        }
        else
        {
            isGrouded = false;
            _terrainNormal = Vector3.up;
        }

        return isGrouded;
    }
    
    private void CheckFalling()
    {
        if (!CheckGround())
        {
            _animator.SetBool(Constants.IsFalling, true);
            return;
        }

        if (CheckGround())
        {
            _animator.SetBool(Constants.IsFalling, false);
        }
        
    }
    
    public void OnMove(InputValue val)
    {
        _inputVector = val.Get<Vector2>();
    }
    
    public void OnSprint()
    {
        if (CheckGround() && !_isSwimming)
        {
            _isRunning = !_animator.GetBool(Constants.IsRunning);
            _animator.SetBool(Constants.IsRunning, _isRunning);
        }
    }

    public void OnJump()
    {
        if (CheckGround() && !_isSwimming)
        {
            //StartCoroutine(DelayCheckGrounded());
            _body.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            _animator.SetBool(Constants.IsFalling, true);
            
        }
    }

    private void CheckStairs()
    {
        if(!_isMoving)
            return;
        
        Vector3 hPos = transform.position;
        Vector3 vPos = hPos;
        vPos += transform.forward * (hStairsOff + _capsuleCollider.radius);
        vPos += transform.up * vStairsOff;

        //Debug
        Debug.DrawRay(hPos, transform.forward * (hStairsOff + _capsuleCollider.radius), Color.blue, 1f);
        Debug.DrawRay(vPos, transform.up * (vStairsOff * -1), Color.green, 1f);
        
        //Check stair on front
        if (Physics.Raycast(hPos, transform.forward, hStairsOff))
        {
            print("Obstacle forward");
            if (Physics.Raycast(vPos, transform.up * -1, vStairsOff))
            {
                print("Stairs founded");
                _body.MovePosition(_body.position + (transform.up * (stairsUpSpeed * Time.deltaTime)));
                return;
            }
        }
        
        //Check stair on + 45 
        Vector3 vPosRight = hPos;
        vPosRight += (transform.forward + transform.right) * (hStairsOff + _capsuleCollider.radius);
        vPosRight += transform.up * vStairsOff;
        if (Physics.Raycast(hPos, transform.forward + transform.right, hStairsOff))
        {
            print("Obstacle forward");
            if (Physics.Raycast(vPosRight, transform.up * -1, vStairsOff))
            {
                print("Stairs founded");
                _body.MovePosition(_body.position + (transform.up * (stairsUpSpeed * Time.deltaTime)));
                return;
            }
        }
        
        //Check stair on -45
        Vector3 vPosLeft = hPos;
        vPosLeft += (transform.forward + (-1 * transform.right)) * (hStairsOff + _capsuleCollider.radius);
        vPosLeft += transform.up * vStairsOff;
        if (Physics.Raycast(hPos, transform.forward + (transform.right * -1) , hStairsOff))
        {
            print("Obstacle forward");
            if (Physics.Raycast(vPosLeft, transform.up * -1, vStairsOff))
            {
                print("Stairs founded");
                _body.MovePosition(_body.position + (transform.up * (stairsUpSpeed * Time.deltaTime)));
                return;
            }
        }
    }

    private void OnRecover()
    {
        HealthManager.Instance.InvokeRecover();
    }

    public bool IsMoving()
    {
        return _isMoving;
    }
    
    public bool IsSwimming()
    {
        return _isSwimming;
    }
    
    public bool IsRunning()
    {
        return _isRunning;
    }
    
    public void SetIsInsideWater(bool val)
    {
        _isInsideWater = val;
    }
}
