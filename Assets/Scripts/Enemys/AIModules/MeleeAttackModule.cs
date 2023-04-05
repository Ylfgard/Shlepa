using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using PlayerController;
using System;

namespace Enemys.AIModules
{
    public class MeleeAttackModule : MonoBehaviour
    {
        public Action AttackFinished;

        [Header ("Attack Parameters")]
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _attackDistance;
        [SerializeField] protected float _attackDamageArea;

        protected Transform _transform;
        protected AnimationController _animationController;
        protected NavMeshAgent _agent;
        protected Transform _target;
        protected Player _player;
        protected bool _isAttacking;

        public virtual void Initialize(Transform myTransform, AnimationController animationController, NavMeshAgent agent, Player player)
        {
            _transform = myTransform;
            _agent = agent;
            _player = player;
            _target = player.Mover.Transform;
            _animationController = animationController;
            _animationController.CallBack += ActivateAttack;
        }

        public bool TryAttack()
        {
            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
            }
            return (_isAttacking);
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

        protected virtual void ActivateAttack()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _attackDamageArea)
                _player.Parameters.TakeDamage(_damage);
            _agent.isStopped = false;
            AttackFinished?.Invoke();
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