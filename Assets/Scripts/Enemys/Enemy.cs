using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using PlayerController;
using System;
using System.Collections;

namespace Enemys
{
    public abstract class Enemy : MonoBehaviour
    {
        public Action<Enemy> SendDeath;
        public Action SendAttack;
        public Action<int> TakedDamage;

        [Header("Name")]
        [SerializeField] protected string _name;

        [Header ("Balance")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected List<WeakPoint> _weakPoints;

        [Header("Parameters")]
        [SerializeField] protected AnimationController _animationController;
        [SerializeField] protected float _deathDelay;

        protected Transform _transform;
        protected NavMeshAgent _agent;
        protected Player _player;
        protected int _curHealth;
        protected bool _isAlive;

        public string Name => _name;
        public List<WeakPoint> WeakPoints => _weakPoints;
        public AnimationController AnimationController => _animationController;
        public Transform Transform => _transform;
        public NavMeshAgent Agent => _agent;
        public Player Player => _player;

        protected virtual void Awake()
        {
            _transform = transform;
            _agent = GetComponent<NavMeshAgent>();
            _player = Player.Instance;
            EnemyKeeper.Instance.AddEnemy(this);
            foreach (WeakPoint weakPoint in _weakPoints)
                weakPoint.Initialize(this);
        }

        protected virtual void Attack()
        {
            SendAttack?.Invoke();
        }

        public virtual void Initialize(Vector3 position)
        {
            _transform.position = position;
            _curHealth = _maxHealth;
            _agent.isStopped = false;
            _isAlive = true;
        }

        public virtual void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_curHealth <= 0)
                Death();
            TakedDamage?.Invoke(GetHealthPercent());
        }

        public int GetHealthPercent()
        {
            float percent = ((float)_curHealth / _maxHealth) * 100;
            return Mathf.RoundToInt(percent);
        }

        public virtual void Death()
        {
            if (_isAlive == false) return; 
            StopAllCoroutines();
            StartCoroutine(DeathDelay());
        }

        protected virtual IEnumerator DeathDelay()
        {
            _animationController.SetTrigger("Death");
            _agent.isStopped = true;
            _isAlive = false;
            yield return new WaitForSeconds(_deathDelay);
            SendDeath?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}