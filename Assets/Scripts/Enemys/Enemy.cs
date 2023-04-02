using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using PlayerController;
using System.Collections;
using System;

namespace Enemys
{
    public abstract class Enemy : MonoBehaviour
    {
        protected const float _spawnHight = 10;
        protected const float _fallSpeed = 20;

        public Action<Enemy> SendDeath;

        [Header ("Spawn Parameters")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] protected float _landingAreaRadius;
        [SerializeField] protected int _landingDamage;

        [Header ("Balance")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected float _speed;
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _attackDistance;
        [SerializeField] protected List<WeakPoint> _weakPoints;

        [Header("Parameters")]
        [SerializeField] protected AnimationController _animationController;

        protected Transform _transform;
        protected NavMeshAgent _agent;
        protected Player _player;
        protected bool _isAttacking;
        protected int _curHealth;
        protected Transform _target;
        protected bool _isLanding;

        public List<WeakPoint> WeakPoints => _weakPoints;
        public float LandingAreaRadius => _landingAreaRadius;

        protected virtual void Awake()
        {
            _transform = transform;
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _speed;
            _agent.angularSpeed = 360;
            _agent.acceleration = 100;
            foreach (WeakPoint weakPoint in _weakPoints)
                weakPoint.Initialize(this);
        }

        protected virtual void Start()
        {
            _player = Player.Instance;
            _target = _player.Mover.Transform;
            EnemyKeeper.Instance.AddEnemy(this);
        }

        public virtual void Initialize(Vector3 position)
        {
            position.y += _spawnHight;
            _transform.position = position;
            _curHealth = _maxHealth;
            _isAttacking = false;
            if (_landingAreaRadius > 0)
            {
                _isLanding = true;
                _agent.enabled = false;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_isLanding)
            {
                CheckLanding();
                return;
            }
        }

        protected void CheckLanding()
        {
            if (Physics.Raycast(_transform.position, Vector3.down, 2f, _groundLayer))
            {
                Landing();
            }
            else
            {
                Vector3 newPos = _transform.position;
                newPos.y -= _fallSpeed * Time.fixedDeltaTime;
                _transform.position = newPos;
            }
        }

        protected void Landing()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_landingDamage);
            _isLanding = false;
            _agent.enabled = true;
        }

        protected virtual void Attack()
        {
            _isAttacking = true;
            _animationController.SetTrigger("Attack");
            StartCoroutine(PrepareAttack());
        }

        protected IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(_attackDelay);
            _isAttacking = false;
        }

        public virtual void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_curHealth <= 0)
                Death();
        }

        public virtual void Death()
        {
            StopAllCoroutines();
            SendDeath?.Invoke(this);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
            Gizmos.DrawWireSphere(transform.position, _landingAreaRadius);
        }
#endif
    }
}