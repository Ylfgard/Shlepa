using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using PlayerController;
using System.Threading.Tasks;

namespace Enemys
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header ("Balance")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected float _speed;
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _attackDistance;
        [SerializeField] protected List<WeakPoint> _weakPoints;

        [Header("Parameters")]
        [SerializeField] protected float _triggerDistance;
        [SerializeField] protected AnimationController _animationController;
        
        protected Transform _transform;
        protected NavMeshAgent _agent;
        protected Player _player;
        protected bool _isAttacking;
        protected bool _isActive;
        protected int _curHealth;
        protected Transform _target;

        public List<WeakPoint> WeakPoints => _weakPoints;

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
            Initialize(_transform.position, true);
        }

        public virtual void Initialize(Vector3 position, bool active)
        {
            _transform.position = position;
            _curHealth = _maxHealth;
            _isActive = active;
            _isAttacking = false;
        }

        protected virtual void Attack()
        {
            _isAttacking = true;
            _animationController.SetTrigger("Attack");
            PrepareAttack();
        }

        protected async void PrepareAttack()
        {
            await Task.Delay(Mathf.RoundToInt(_attackDelay * 1000));
            _isAttacking = false;
        }

        public virtual void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_isActive == false) _isActive = true;
            if (_curHealth <= 0)
                Death();
        }

        public virtual void Death()
        {
            Debug.Log("Im DEAD! " + gameObject.name);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _triggerDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
#endif
    }
}