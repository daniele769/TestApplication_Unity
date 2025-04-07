using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum AIState
{
    Patrol = 0,
    Chase = 1,
    Attack = 2
}
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    
    private Vector3 _patrolDestination;
    private bool _destinationFounded;
    private bool _isWaitingForNextPatrol;
    private AIState _currentAIState;
    private bool _playerIsInsideRangeOfView;
    private Transform _player;
    private Coroutine _waitCoroutine;
    private float _defaultSpeed;
    private float _defaultAcceleration;
    private bool _isAttacking;
    private int _hitTaken = 0;
    private AudioSource _audioSource;
    private bool _isChaseAlreadyStarted;
    private bool _isDeath;

    [SerializeField] 
    private float _distanceForNextPatrolPosition = 15f;
    
    [SerializeField] 
    private float _distanceOffset = 5f;

    [SerializeField] 
    private float maxDistanceFromRandomPos = 1f;
    
    [SerializeField] 
    private float maxWaitTimeForNextPatrol = 6f;
    
    [SerializeField] 
    private float minWaitTimeForNextPatrol = 3f;

    [SerializeField] 
    private float distancePlayerAlwaysSeen;
    
    [SerializeField] 
    private float viewDistance = 10f;
    
    [SerializeField] 
    private float lostAggroDistance = 35f;
    
    [SerializeField] 
    private float fov = 90f;
    
    [SerializeField] 
    private float chaseSpeed = 8f;
    
    [SerializeField] 
    private float chaseAcceleration = 10f;
    
    [SerializeField] 
    private float attackDistance = 2f;
    
    [SerializeField] 
    private float dealDamageMaxDistance = 4f;
    
    [SerializeField] 
    private float damage = 30f;
    
    [SerializeField] 
    private int hitToDie = 2;

    [SerializeField] 
    private AudioClip idleClip;
    
    [SerializeField] 
    private AudioClip chaseClip;
    
    [SerializeField] 
    private AudioClip attackClip;
    
    [SerializeField] 
    private AudioClip deathClip;
    
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _currentAIState = AIState.Patrol;

        _defaultAcceleration = _agent.acceleration;
        _defaultSpeed = _agent.speed;
    }
    
    
    void Update()
    {
        if (_isDeath)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Death") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !_audioSource.isPlaying)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        
        switch (_currentAIState)
        {
            case AIState.Patrol: Patrol(); break;
            case AIState.Chase: Chase(); break;
            case AIState.Attack: Attack(); break; 
        }
        
        if (_agent.velocity.sqrMagnitude > 0)
        {
            if(!_animator.GetBool(Constants.IsMoving))
                _animator.SetBool(Constants.IsMoving, true);
        }
        else
        {
            if(_animator.GetBool(Constants.IsMoving))
                _animator.SetBool(Constants.IsMoving, false);
        }
    }
    
    private void Patrol()
    {
        if (_playerIsInsideRangeOfView && IsPlayerVisible())
        {
            _currentAIState = AIState.Chase;
            return;   
        }

        if (_audioSource.resource != idleClip)
        {
            _audioSource.Stop();
            _audioSource.resource = idleClip;
        }
        else if (!_audioSource.isPlaying)
        {
            _audioSource.PlayDelayed(Random.Range(8, 16));
        }
        
        if (_destinationFounded && _agent.velocity.sqrMagnitude == 0 && !_isWaitingForNextPatrol)
        {
            _waitCoroutine = StartCoroutine(WaitForNextPatrol());
            _isWaitingForNextPatrol = true;
        }
        else if (!_destinationFounded && !_isWaitingForNextPatrol)
        {
            CalcRandomPatrolPosition();
            if (_destinationFounded)
            {
                _agent.SetDestination(_patrolDestination);
            }
        }
    }

    private void Chase()
    {
        if (!_animator.GetBool(Constants.IsRunning))
        {
            _animator.SetBool(Constants.IsRunning, true);
            _agent.speed = chaseSpeed;
            _agent.acceleration = chaseAcceleration;
            if (!_isChaseAlreadyStarted)
            {
                _isChaseAlreadyStarted = true;
                _audioSource.Stop();
                _audioSource.resource = chaseClip;
                _audioSource.Play();
            }
        }
            
        
        _agent.SetDestination(_player.position);
        if (Vector3.Distance(transform.position, _player.position) <= attackDistance)
        {
            _currentAIState = AIState.Attack;
            return;
        }

        if (Vector3.Distance(transform.position, _player.position) > lostAggroDistance)
        {
            _agent.speed = _defaultSpeed;
            _agent.acceleration = _defaultAcceleration;
            _animator.SetBool(Constants.IsRunning, false);
            _currentAIState = AIState.Patrol;
            _isChaseAlreadyStarted = false;
        }
    }

    private void Attack()
    {
        _agent.SetDestination(_player.position);
        if (!_isAttacking && Vector3.Distance(transform.position, _player.position) > attackDistance)
        {
            _currentAIState = AIState.Chase;
            return;
        }
        
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 7f);
        
        if (!_isAttacking)
        {
            _audioSource.Stop();
            _audioSource.resource = attackClip;
            _audioSource.Play();
            _animator.SetTrigger(Constants.AttackTrigger);
            _isAttacking = true;
            return;
        }

        AnimatorStateInfo animatorState = _animator.GetCurrentAnimatorStateInfo(0);
        if (!animatorState.IsName("Attack"))
        {
            _isAttacking = false;
        }
    }

    private void CalcRandomPatrolPosition()
    {
        float radius = Random.Range(_distanceForNextPatrolPosition - _distanceOffset, _distanceForNextPatrolPosition + _distanceOffset);
        NavMeshHit hit;
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * radius;
            randomPos.y = transform.position.y;
            
            if (NavMesh.SamplePosition(randomPos, out hit, maxDistanceFromRandomPos, NavMesh.AllAreas))
            {
                print("*** Destination Founded !!!! ***");
                _patrolDestination = hit.position;
                _destinationFounded = true;
                break;
            }
            print("Destination not founded");
        }
    }

    private IEnumerator WaitForNextPatrol()
    {
        //_animator.SetBool(Constants.IsChasing, false);
        float duration = Random.Range(minWaitTimeForNextPatrol, maxWaitTimeForNextPatrol);
        yield return new WaitForSeconds(duration);
        _destinationFounded = false;
        _isWaitingForNextPatrol = false;
    }

    private bool IsPlayerVisible()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= 
            distancePlayerAlwaysSeen)
        {
            return true;
        }

        Vector3 directionToPlayer = _player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (directionToPlayer.magnitude < viewDistance && angle < fov / 2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer.normalized * viewDistance, out RaycastHit hit))
            {
                //Check if there are obstacle between player and enemy
                if (hit.transform.CompareTag("Player"))
                {
                    _currentAIState = AIState.Chase;
                    StopCoroutine(_waitCoroutine);
                    return true;
                }
            }
        }

        return false;
    }

    //called by animation event
    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, _player.position) <= dealDamageMaxDistance)
        {
            HealthManager.Instance.InvokeDamage(damage);
        }
    }

    public void TakeDamage()
    {
        if(_isDeath)
            return;
        
        _hitTaken++;
        if (_hitTaken >= hitToDie)
        {
            _isDeath = true;
            _animator.SetTrigger(Constants.IsDeath);
            _audioSource.Stop();
            _audioSource.resource = deathClip;
            _audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            print("Player inside range of view");
            _playerIsInsideRangeOfView = true;
            _player = other.transform;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            _playerIsInsideRangeOfView = true;
            if(_player == null)
                _player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            print("Player outside range of view");
            _playerIsInsideRangeOfView = false;
        }
        
    }
}
