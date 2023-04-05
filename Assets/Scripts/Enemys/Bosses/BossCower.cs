using Enemys.Cowers;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Enemys.Bosses
{
    public abstract class BossCower : BossRange
    {
        [Header("Cower")]
        [SerializeField] protected float _distanceFromTarget;
        [SerializeField] protected float _changeCowerDistance;

        protected CowerKeeper _cowerKeeper;
        protected Cower _curCower;
        protected bool _moveToCover;
        protected bool _moveToAlternativeCower;
        protected bool _inCower;

        protected override void Start()
        {
            base.Start();
            _cowerKeeper = CowerKeeper.Instance;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
                else
                    MoveToAttack();
            }
            else
            {
                if (Vector3.Distance(_agent.destination, _transform.position) <= 1f) _inCower = true;
                TakeCover();
            }
        }

        protected override void Attack()
        {
            _moveToCover = false;
            _moveToAlternativeCower = false;
            _inCower = false;
            base.Attack();
            MoveToAttack();
        }

        protected override void FastShot()
        {
            base.FastShot();
            TakeCover();
        }

        protected override IEnumerator Shot()
        {
            yield return base.Shot();
            TakeCover();
        }

        protected void TakeCover()
        {
            if (Vector3.Distance(_transform.position, _target.position) < _changeCowerDistance &&
                _inCower && _moveToAlternativeCower == false) _moveToCover = false;

            if (_moveToCover) return;
            _inCower = false;
            _moveToCover = true;
            Vector3 cowerOffset = (_transform.position - _target.position).normalized * _distanceFromTarget;
            Cower cower = _cowerKeeper.GetNearestShelter(_target.position + cowerOffset);
            if (_curCower != cower)
            {
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _curCower = cower;
            }
            else
            {
                cower = _cowerKeeper.GetNearestShelter(_target.position - cowerOffset);
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _moveToAlternativeCower = true;
            }
        }

        protected void MoveToAttack()
        {
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
            _curCower = null;
        }

        public override void TakeDamage(int value)
        {
            base.TakeDamage(value);
            if (_inCower)
            {
                _moveToCover = false;
                _moveToAlternativeCower = false;
                _inCower = false;
            }
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _changeCowerDistance);
        }
#endif
    }
}