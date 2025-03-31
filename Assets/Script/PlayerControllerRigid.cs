using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRigid : MonoBehaviour
{
    private Rigidbody _body;
    private bool _isMoving;
    private bool _isRunning;
    private Vector2 _inputVector;
    private Vector3 _terrainNormal;
    private Animator _animator;
    private CapsuleCollider _capsuleCollider;
    private PlayerInput _playerInput;
    
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
    private Camera myCamera;
    
    void Start()
    {
        _body = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _playerInput = GetComponent<PlayerInput>();
        
        _terrainNormal = Vector3.up;
        PlayerInputState.CurrentControlScheme = _playerInput.currentControlScheme;
        PlayerInputState.CurrentActionMap = _playerInput.currentActionMap.name;
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
    }

    private void MovePlayer()
    {
        if (_inputVector != Vector2.zero)
        {
            _isMoving = true;
            _animator.SetBool("IsMoving", true);
            Vector3 hVector = myCamera.transform.right;
            Vector3 vVector = myCamera.transform.forward;
            hVector.y = 0;
            vVector.y = 0;
        
            Vector3 moveVector = hVector * _inputVector.x + vVector * _inputVector.y;
            moveVector = moveVector.normalized;
            UpdatePlayerRotation(moveVector);
            
            Vector3 moveVectorOnGround = Vector3.ProjectOnPlane(moveVector, _terrainNormal).normalized;
            if(_isRunning)
                _body.MovePosition(_body.position + moveVectorOnGround * (Time.fixedDeltaTime * runSpeed));
            else
                _body.MovePosition(_body.position + moveVectorOnGround * (Time.fixedDeltaTime * walkSpeed));
            
            return;
        }
        _animator.SetBool("IsMoving", false);
        _animator.SetBool("IsRunning", false);
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

    private bool CheckGround()
    {
        bool isGrouded;
        RaycastHit hit;
        
        Vector3 pos = transform.position;
        pos.y += 0.25f;
        if(Physics.SphereCast(pos, 0.2f, Vector3.down, out hit, 0.5f + fallingOffset))
        {
            isGrouded = true;
            _terrainNormal = hit.normal;
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
            _animator.SetBool("IsFalling", true);
            return;
        }

        if (CheckGround())
        {
            _animator.SetBool("IsFalling", false);
        }
        
    }
    
    public void OnMove(InputValue val)
    {
        _inputVector = val.Get<Vector2>();
    }
    
    public void OnSprint()
    {
        if (CheckGround())
        {
            _isRunning = !_animator.GetBool("IsRunning");
            _animator.SetBool("IsRunning", _isRunning);
        }
    }

    public void OnJump()
    {
        if (CheckGround())
        {
            //StartCoroutine(DelayCheckGrounded());
            _body.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            _animator.SetBool("IsFalling", true);
            
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
}
