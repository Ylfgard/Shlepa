using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using PlayerController;
using System;

namespace Enemys.AIModules
{
    public class AttackModule : AIModule
    {
        public Action AttackFinished;

        [Header("Attack Parameters")]
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] [Range (1.5f, 100)] protected float _attackDistance;
        [SerializeField] protected float _attackDamageArea;

        protected Transform _transform;
        protected AnimationController _animationController;
        protected NavMeshAgent _agent;
        protected Transform _target;
        protected Player _player;
        protected bool _isAttacking;

        public bool AttackReady => !_isAttacking;

        public virtual void Initialize(Enemy enemy)
        {
            _transform = enemy.Transform;
            _agent = enemy.Agent;
            _player = enemy.Player;
            _target = _player.Mover.Transform;
            _animationController = enemy.AnimationController;
            _animationController.CallBack += ActivateAttack;
            _isAttacking = false;
        }

        public virtual bool TryAttack()
        {
            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    return Attack();
                else
                    return false;
            }
            return false;
        }

        protected virtual bool Attack()
        {
            _isAttacking = true;
            _animationController.SetTrigger("Attack");
            StartCoroutine(PrepareAttack());
            _agent.isStopped = true;
            return true;
        }

        protected IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(_attackDelay);
            _isAttacking = false;
        }

        protected virtual void ActivateAttack()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _attackDamageArea)
                _player.Parameters.TakeDamage(_damage);
            _agent.isStopped = false;
            AttackFinished?.Invoke();
        }

        public void ForcedActivateAttack()
        {
            ActivateAttack();
        }

        public override void Deactivate(Enemy enemy)
        {
            base.Deactivate(enemy);
            _isAttacking = false;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);

            Gizmos.color = Color.red + Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackDamageArea);
        }
#endif
    }
}