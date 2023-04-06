using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using PlayerController;
using System;
using LevelMechanics.EnemySpawners;

namespace Enemys
{
    public abstract class Enemy : MonoBehaviour
    {
        public Action<Enemy> SendDeath;
        public Action SendAttack;
        public Action<int> TakedDamage;

        [Header ("Balance")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected List<WeakPoint> _weakPoints;

        [Header("Parameters")]
        [SerializeField] protected AnimationController _animationController;

        protected Transform _transform;
        protected NavMeshAgent _agent;
        protected Player _player;
        protected int _curHealth;

        public List<WeakPoint> WeakPoints => _weakPoints;
        public AnimationController AnimationController => _animationController;
        public Transform Transform => _transform;
        public NavMeshAgent Agent => _agent;
        public Player Player => _player;

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

        protected virtual void Attack()
        {
            SendAttack?.Invoke();
        }

        public virtual void Initialize(Vector3 position)
        {
            _transform.position = position;
            _curHealth = _maxHealth;
        }

        public virtual void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_curHealth <= 0)
                Death();
            else
                TakedDamage?.Invoke(GetHealthPercent());
        }

        protected int GetHealthPercent()
        {
            float percent = ((float)_curHealth / _maxHealth) * 100;
            return Mathf.RoundToInt(percent);
        }

        public virtual void Death()
        {
            StopAllCoroutines();
            SendDeath?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}