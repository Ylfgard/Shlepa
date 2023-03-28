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

        public List<WeakPoint> WeakPoints => _weakPoints;

        protected virtual void Awake()
        {
            _transform = transform;
            _agent = GetComponent<NavMeshAgent>();
            foreach (WeakPoint weakPoint in _weakPoints)
                weakPoint.Initialize(this);
        }

        protected virtual void Start()
        {
            _player = Player.Instance;
            EnemyKeeper.Instance.AddEnemy(this);
        }

        public virtual void Initialize(bool active)
        {
            _curHealth = _maxHealth;
        }

        protected virtual void Attack()
        {
            _animationController.SetTrigger("Attack");
            _isAttacking = true;
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
            if (_curHealth <= 0)
                Death();
        }

        public virtual void Death()
        {
            Debug.Log("Im DEAD! " + gameObject.name);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _triggerDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
#endif
    }
}